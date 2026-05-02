using Microsoft.EntityFrameworkCore;
using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Domain.Accounts.Errors;
using VcsDevelop.Infrastructure.DbContexts;

namespace VcsDevelop.Infrastructure.Repositories.Accounts;

public sealed class AccountRepository : BaseRepository, IAccountRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    public AccountRepository(VcsDevelopDbContext dbContext)
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

    public async Task<Account?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accounts
            .SingleOrDefaultAsync(account => account.Email == email, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Account> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

        if (account is null)
        {
            throw AccountNotFound.ById(id);
        }

        return account;
    }

    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);

        var account = await _dbContext.Accounts
            .SingleOrDefaultAsync(
                account => account.Email == email,
                cancellationToken)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw AccountNotFound.ByEmail(email);
        }

        return account;
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
