using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface IStagingAreaRepository
{
    Task AddOrReplaceAsync(StagedFileEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StagedFileEntry>> GetAllAsync(
        Guid documentId,
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task ClearAsync(Guid documentId, Guid accountId, CancellationToken cancellationToken = default);
}
