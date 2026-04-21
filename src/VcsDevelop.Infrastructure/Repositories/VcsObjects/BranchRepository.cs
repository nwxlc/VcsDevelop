using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;
using Branch = VcsDevelop.Domain.VcsObjects.Branch;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class BranchRepository : BaseRepository, IBranchRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public BranchRepository(VcsDevelopDbContext dbContext)
        : base(dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task<Branch?> FindByDocumentAndNameAsync(
        Guid documentId,
        string branchName,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Branches
            .SingleOrDefaultAsync(
                branch => branch.DocumentId == documentId && branch.Name == branchName,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task SetAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        if (_dbContext.ChangeTracker.Entries<Branch>().All(entry => entry.Entity.Id != branch.Id))
        {
            var existingBranch = await _dbContext.Branches
                .SingleOrDefaultAsync(item => item.Id == branch.Id, cancellationToken)
                .ConfigureAwait(false);

            if (existingBranch is null)
            {
                _dbContext.Branches.Add(branch);
            }
        }

        await CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
