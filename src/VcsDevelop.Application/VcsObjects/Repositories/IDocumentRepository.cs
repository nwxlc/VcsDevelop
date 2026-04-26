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

    Task SetAsync(Document document, CancellationToken cancellationToken = default);
}
