using System.Text.Json;
using StackExchange.Redis;

namespace GitDevelop.Infrastructure.Auth;

public sealed class RedisRefreshTokenRepository : IRefreshTokenRepository
{
    private const string TokenKeyPrefix = "refresh_token:";
    private const string UserTokensKeyPrefix = "user_tokens:";

    private readonly IConnectionMultiplexer _redis;

    public RedisRefreshTokenRepository(IConnectionMultiplexer redis)
    {
        ArgumentNullException.ThrowIfNull(redis);

        _redis = redis;
    }

    public async Task SetAsync(RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var tokenKey = GetTokenKey(refreshToken.TokenHash);
        var userKey = GetUserTokensKey(refreshToken.UserId);

        var tokenExpiresAt = refreshToken.ExpiresAt - DateTimeOffset.UtcNow;
        if (tokenExpiresAt <= TimeSpan.Zero)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(refreshToken);

        var score = (double)refreshToken.ExpiresAt.ToUnixTimeSeconds();

        try
        {
            var database = _redis.GetDatabase();

            var transaction = database.CreateTransaction();

            _ = transaction.StringSetAsync(tokenKey, payload, tokenExpiresAt);
            _ = transaction.SortedSetAddAsync(userKey, refreshToken.TokenHash, score);

            var commited = await transaction
                .ExecuteAsync()
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!commited)
            {
                throw new InvalidOperationException("Redis transaction failed");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to store refresh token", ex);
        }
    }

    public async Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);

        var tokenHash = RefreshToken.ComputeHash(token);
        var tokenKey = GetTokenKey(tokenHash);

        try
        {
            var database = _redis.GetDatabase();

            var value = await database
                .StringGetAsync(tokenKey)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!value.HasValue)
            {
                return null;
            }

            return JsonSerializer.Deserialize<RefreshToken>(value.ToString());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to retrieve refresh token", ex);
        }
    }

    public async Task RemoveAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var tokenKey = GetTokenKey(refreshToken.TokenHash);
        var userTokensKey = GetUserTokensKey(refreshToken.UserId);

        try
        {
            var db = _redis.GetDatabase();

            var token = await GetAsync(refreshToken.TokenHash, cancellationToken);
            if (token is null)
            {
                return;
            }

            var transaction = db.CreateTransaction();

            _ = transaction.KeyDeleteAsync(tokenKey);
            _ = transaction.SortedSetRemoveAsync(userTokensKey, refreshToken.TokenHash);

            var committed = await transaction
                .ExecuteAsync()
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!committed)
            {
                throw new InvalidOperationException("Failed to remove refresh token in Redis");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to remove refresh token", ex);
        }
    }

    public async Task RemoveAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userTokensKey = GetUserTokensKey(userId);

        try
        {
            var database = _redis.GetDatabase();

            var hashes = await database
                .SortedSetRangeByRankAsync(userTokensKey)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (hashes.Length == 0)
            {
                return;
            }

            var transaction = database.CreateTransaction();

            foreach (var hash in hashes)
            {
                var tokenKey = GetTokenKey(hash.ToString());
                _ = transaction.KeyDeleteAsync(tokenKey);
            }

            _ = transaction.KeyDeleteAsync(userTokensKey);

            var committed = await transaction.ExecuteAsync()
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!committed)
            {
                throw new InvalidOperationException("Failed to remove all user refresh tokens in Redis");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to remove all user tokens", ex);
        }
    }

    private static string GetTokenKey(string tokenHash) => $"{TokenKeyPrefix}{tokenHash}";
    private static string GetUserTokensKey(Guid userId) => $"{UserTokensKeyPrefix}{userId}";
}
