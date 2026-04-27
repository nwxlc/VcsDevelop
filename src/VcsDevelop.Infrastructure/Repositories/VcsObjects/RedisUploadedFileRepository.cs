using System.Text.Json;
using StackExchange.Redis;
using VcsDevelop.Application.VcsObjects.Files.Models;
using VcsDevelop.Application.VcsObjects.Repositories;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class RedisUploadedFileRepository : IUploadedFileRepository
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IDatabase _database;

    public RedisUploadedFileRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        ArgumentNullException.ThrowIfNull(connectionMultiplexer);

        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<UploadedFileReference?> FindByIdAsync(Guid uploadId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var value = await _database.StringGetAsync(BuildKey(uploadId)).ConfigureAwait(false);
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<UploadedFileReference>(value.ToString(), JsonSerializerOptions);
    }

    public async Task SetAsync(UploadedFileReference reference, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reference);
        cancellationToken.ThrowIfCancellationRequested();

        var expiration = reference.ExpiresAt - DateTime.UtcNow;
        if (expiration <= TimeSpan.Zero)
        {
            expiration = TimeSpan.FromMinutes(1);
        }

        var payload = JsonSerializer.Serialize(reference, JsonSerializerOptions);
        await _database.StringSetAsync(
                BuildKey(reference.UploadId),
                payload,
                expiry: expiration)
            .ConfigureAwait(false);
    }

    public async Task RemoveAsync(Guid uploadId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _database.KeyDeleteAsync(BuildKey(uploadId)).ConfigureAwait(false);
    }

    private static string BuildKey(Guid uploadId) => $"uploads:{uploadId:N}";
}
