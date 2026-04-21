using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;
using Tree = VcsDevelop.Domain.VcsObjects.Tree;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class TreeRepository : BaseRepository, ITreeRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public TreeRepository(VcsDevelopDbContext dbContext)
        : base(dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task<Tree?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Trees
            .SingleOrDefaultAsync(tree => tree.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task SetAsync(Tree tree, CancellationToken cancellationToken = default)
    {
        if (_dbContext.ChangeTracker.Entries<Tree>().All(entry => entry.Entity.Id != tree.Id))
        {
            _dbContext.Trees.Add(tree);
        }

        await CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
