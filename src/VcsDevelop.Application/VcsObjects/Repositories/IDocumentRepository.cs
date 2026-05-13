using Document = VcsDevelop.Domain.VcsObjects.Document;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface IDocumentRepository
{
    Task<Document?> FindByIdAsync(
        Guid id,
        Guid ownerId,
        CancellationToken cancellationToken = default);

    Task<Document?> FindByNameAndOwnerAsync(
        string name,
        Guid ownerId,
        CancellationToken cancellationToken);

    Task<Document> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Document>> GetAllByOwnerIdAsync(
        Guid ownerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> GetCountByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default);

    Task SetAsync(Document document, CancellationToken cancellationToken = default);
}
