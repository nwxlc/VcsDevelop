using System.Text.Json;
using StackExchange.Redis;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Repositories;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class RedisStagingAreaRepository : IStagingAreaRepository
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IDatabase _database;

    public RedisStagingAreaRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        ArgumentNullException.ThrowIfNull(connectionMultiplexer);

        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task AddOrReplaceAsync(StagedFileEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(entry.DocumentId, entry.AccountId);
        var payload = JsonSerializer.Serialize(entry, JsonSerializerOptions);

        await _database.HashSetAsync(
                key,
                entry.RepositoryPath,
                payload)
            .ConfigureAwait(false);

        await _database.KeyExpireAsync(
                key,
                TimeSpan.FromHours(24))
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<StagedFileEntry>> GetAllAsync(
        Guid documentId,
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(documentId, accountId);
        var values = await _database.HashValuesAsync(key).ConfigureAwait(false);

        return values
            .Where(redisValue => !redisValue.IsNullOrEmpty)
            .Select(redisValue => JsonSerializer.Deserialize<StagedFileEntry>(
                redisValue.ToString(),
                JsonSerializerOptions))
            .Where(entry => entry is not null)
            .Select(entry => entry!)
            .ToArray();
    }

    public async Task ClearAsync(Guid documentId, Guid accountId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _database.KeyDeleteAsync(BuildKey(documentId, accountId)).ConfigureAwait(false);
    }

    private async Task<List<StagedFileEntry>> ReadEntriesAsync(string key)
    {
        var value = await _database.StringGetAsync(key).ConfigureAwait(false);
        if (value.IsNullOrEmpty)
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<StagedFileEntry>>(value.ToString(), JsonSerializerOptions) ?? [];
    }

    private static string BuildKey(Guid documentId, Guid accountId)
    {
        return $"staging:{accountId:N}:{documentId:N}";
    }
}
