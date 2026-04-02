using GitDevelop.Application.Accounts.Repositories;
using GitDevelop.Domain.Accounts;
using GitDevelop.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GitDevelop.Infrastructure.Repositories.Accounts;

public sealed class AccountRepository : BaseRepository, IAccountRepository
{
    private readonly GitDevelopDbContext _dbContext;

    public AccountRepository(GitDevelopDbContext dbContext)
        : base(dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task<Account?> FindByIdAsync(Guid? id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accounts
            .SingleOrDefaultAsync(account => account.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task SetAsync(Account account, CancellationToken cancellationToken = default)
    {
        if (_dbContext.ChangeTracker.Entries<Account>().All(a => a.Entity.Id != account.Id))
        {
            _dbContext.Accounts.Add(account);
        }

        await CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
