using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;
using Document = VcsDevelop.Domain.VcsObjects.Document;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public class DocumentRepository : BaseRepository, IDocumentRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public DocumentRepository(VcsDevelopDbContext dbContext)
        : base(dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task<Document?> FindByNameAndOwnerAsync(string name, Guid ownerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Documents
            .SingleOrDefaultAsync(
                document => document.Name == name && document.OwnerId == ownerId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Document?> FindByIdAsync(
        Guid id,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Documents
            .SingleOrDefaultAsync(
                document => document.Id == id && document.OwnerId == ownerId,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task SetAsync(Document document, CancellationToken cancellationToken = default)
    {
        if (_dbContext.ChangeTracker.Entries<Document>().All(doc => doc.Entity.Id != document.Id))
        {
            _dbContext.Documents.Add(document);
        }

        await CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
