using System.Security.Cryptography;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Infrastructure.Options.Tokens;

namespace VcsDevelop.Infrastructure.Auth;

public sealed class RefreshTokenProvider : IRefreshTokenProvider
{
    private readonly ITokenSettings _tokenSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenProvider(
        ITokenSettings tokenSettings,
        IRefreshTokenRepository refreshTokenRepository)
    {
        ArgumentNullException.ThrowIfNull(tokenSettings);

        _tokenSettings = tokenSettings;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Token> CreateRefreshTokenAsync(Account account, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(account);

        var token = CreateRefreshToken();

        var refreshToken = RefreshToken.Create(token, account.Id);

        await _refreshTokenRepository.SetAsync(refreshToken,
                cancellationToken)
            .ConfigureAwait(false);

        return token;
    }

    public async Task RevokeRefreshTokenAsync(string tokenValue, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenValue);

        var tokenData = await _refreshTokenRepository.GetAsync(tokenValue, cancellationToken)
            .ConfigureAwait(false);

        if (tokenData is not null)
        {
            await _refreshTokenRepository.RemoveAsync(tokenData, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public async Task RevokeAllTokensAsync(Guid accountId, CancellationToken cancellationToken)
    {
        await _refreshTokenRepository.RemoveAllUserTokensAsync(accountId, cancellationToken).ConfigureAwait(false);
    }

    private Token CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var tokenValue = Convert.ToBase64String(randomNumber);
        var expiresAt = DateTime.UtcNow.Add(_tokenSettings.RefreshTokenLifetime);

        return Token.Create(tokenValue, expiresAt);
    }
}
