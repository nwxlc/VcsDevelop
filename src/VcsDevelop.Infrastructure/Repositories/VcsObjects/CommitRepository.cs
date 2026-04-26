using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;
using Commit = VcsDevelop.Domain.VcsObjects.Commit;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class CommitRepository : BaseRepository, ICommitRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public CommitRepository(VcsDevelopDbContext dbContext)
        : base(dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task<Commit?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Commits
            .SingleOrDefaultAsync(commit => commit.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task SetAsync(Commit commit, CancellationToken cancellationToken = default)
    {
        if (_dbContext.ChangeTracker.Entries<Commit>().All(entry => entry.Entity.Id != commit.Id))
        {
            _dbContext.Commits.Add(commit);
        }

        await CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
