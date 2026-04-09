using VcsDevelop.Domain.Accounts;

namespace VcsDevelop.Application.Accounts.Repositories;

public interface IAccountRepository
{
    Task<Account?> FindByIdAsync(Guid? id, CancellationToken cancellationToken = default);
    Task SetAsync(Account account, CancellationToken cancellationToken = default);
}
