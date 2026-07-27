using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;
using Tree = VcsDevelop.Domain.VcsObjects.Tree;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class TreeRepository : ITreeRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public TreeRepository(VcsDevelopDbContext dbContext)
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

    public void Add(Tree tree)
    {
        if (_dbContext.ChangeTracker.Entries<Tree>().All(entry => entry.Entity.Id != tree.Id))
        {
            _dbContext.Trees.Add(tree);
        }
    }
    
    public async Task<bool> ExistsAsync(string treeId, CancellationToken cancellationToken = default)
    {
        var existingBranch = await _dbContext.Trees
            .AsNoTracking()
            .AnyAsync(item => item.Id == treeId, cancellationToken)
            .ConfigureAwait(false);
        
        return existingBranch;
    }
}
