using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;
using Branch = VcsDevelop.Domain.VcsObjects.Branch;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class BranchRepository : IBranchRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public BranchRepository(VcsDevelopDbContext dbContext)
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

    public void Add(Branch branch)
    {
        if (_dbContext.ChangeTracker.Entries<Branch>().All(entry => entry.Entity.Id != branch.Id))
        {
            _dbContext.Branches.Add(branch);
        }
    }
}
