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
        var entries = await ReadEntriesAsync(key).ConfigureAwait(false);

        entries.RemoveAll(item => string.Equals(item.RepositoryPath, entry.RepositoryPath, StringComparison.Ordinal));
        entries.Add(entry);

        await WriteEntriesAsync(key, entries).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<StagedFileEntry>> GetAllAsync(
        Guid documentId,
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await ReadEntriesAsync(BuildKey(documentId, accountId)).ConfigureAwait(false);
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

    private async Task WriteEntriesAsync(string key, List<StagedFileEntry> entries)
    {
        var payload = JsonSerializer.Serialize(entries, JsonSerializerOptions);
        await _database.StringSetAsync(
                key,
                payload,
                expiry: TimeSpan.FromHours(24))
            .ConfigureAwait(false);
    }

    private static string BuildKey(Guid documentId, Guid accountId)
    {
        return $"staging:{accountId:N}:{documentId:N}";
    }
}
