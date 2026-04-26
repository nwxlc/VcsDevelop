using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Application.Accounts.Entities.Models;
using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Domain.Accounts.Commands;
using VcsDevelop.Domain.Accounts.Errors;

namespace VcsDevelop.Application.Accounts.CommandHandlers;

public sealed class RegistrationCommandHandler : IRegistrationCommandHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;

    public RegistrationCommandHandler(
        IAccountRepository accountRepository,
        ITokenProvider tokenProvider,
        IRefreshTokenProvider refreshTokenProvider)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(tokenProvider);
        ArgumentNullException.ThrowIfNull(refreshTokenProvider);

        _accountRepository = accountRepository;
        _tokenProvider = tokenProvider;
        _refreshTokenProvider = refreshTokenProvider;
    }

    public async Task<AccountResponse> HandleAsync(RegistrationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingAccount = await _accountRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (existingAccount is not null)
        {
            throw new EmailAlreadyExists();
        }

        var account = Account.Create(request.Name, request.Email, request.Password);

        await _accountRepository.SetAsync(account, cancellationToken)
            .ConfigureAwait(false);

        var token = _tokenProvider.CreateToken(account);

        var refreshToken = await _refreshTokenProvider.CreateRefreshTokenAsync(account, cancellationToken);

        return AccountResponse.Create(account, token, refreshToken);
    }
}
