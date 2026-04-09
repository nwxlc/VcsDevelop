using Document = VcsDevelop.Domain.VcsObjects.Document;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface IDocumentRepository
{
    Task SetAsync(Document document, CancellationToken cancellationToken = default); 
}
