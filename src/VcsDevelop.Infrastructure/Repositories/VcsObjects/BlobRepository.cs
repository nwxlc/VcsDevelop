using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;
using Blob = VcsDevelop.Domain.VcsObjects.Blob;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class BlobRepository : BaseRepository, IBlobRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public BlobRepository(VcsDevelopDbContext dbContext)
        : base(dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task<Blob?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Blobs
            .SingleOrDefaultAsync(blob => blob.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task SetAsync(Blob blob, CancellationToken cancellationToken = default)
    {
        if (_dbContext.ChangeTracker.Entries<Blob>().All(entry => entry.Entity.Id != blob.Id))
        {
            _dbContext.Blobs.Add(blob);
        }

        await CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        var blob = await _dbContext.Blobs
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (blob is null)
        {
            return;
        }

        _dbContext.Blobs.Remove(blob);
        await CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
