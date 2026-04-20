using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Entities.Models;
using VcsDevelop.Application.Accounts.Entities.Queries;
using VcsDevelop.Application.Accounts.Repositories;

namespace VcsDevelop.Application.Accounts.QueryHandlers;

public sealed class GetAccountByIdHandler : IGetAccountByIdHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdHandler(IAccountRepository accountRepository)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);

        _accountRepository = accountRepository;
    }

    public async Task<AccountInfoResponse> HandleAsync(
        GetAccountByIdQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.FindByIdAsync(request.Id, cancellationToken)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new KeyNotFoundException($"Account with id '{request.Id}' was not found.");
        }

        return new AccountInfoResponse
        {
            Id = account.Id,
            Name = account.Name,
            Email = account.Email,
            Bio = account.Bio,
            AvatarUrl = account.AvatarUrl,
            CreatedAt = account.CreatedAt
        };
    }
}
