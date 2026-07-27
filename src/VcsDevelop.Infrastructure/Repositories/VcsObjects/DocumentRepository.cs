using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Domain.VcsObjects.Errors;
using VcsDevelop.Infrastructure.DbContexts;
using Document = VcsDevelop.Domain.VcsObjects.Document;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public class DocumentRepository : IDocumentRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public DocumentRepository(VcsDevelopDbContext dbContext)
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

    public async Task<Document> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.Documents
            .SingleOrDefaultAsync(
                document => document.Id == id,
                cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new DocumentNotFound(id);
        }

        return document;
    }

    public async Task<IEnumerable<Document>> GetAllByOwnerIdAsync(Guid ownerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Documents.AsNoTracking();

        return await queryable
            .Where(document => document.OwnerId == ownerId)
            .OrderByDescending(document => document.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<int> GetCountByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Documents
            .CountAsync(document => document.OwnerId == ownerId, cancellationToken)
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

    public void Add(Document document)
    {
        if (_dbContext.ChangeTracker.Entries<Document>().All(doc => doc.Entity.Id != document.Id))
        {
            _dbContext.Documents.Add(document);
        }
    }
}
