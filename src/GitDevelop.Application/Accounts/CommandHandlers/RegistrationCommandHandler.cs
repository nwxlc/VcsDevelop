using GitDevelop.Application.Accounts.Abstractions;
using GitDevelop.Application.Accounts.Auth;
using GitDevelop.Application.Accounts.Entities;
using GitDevelop.Application.Accounts.Repositories;
using GitDevelop.Core.Application;
using GitDevelop.Domain.Accounts;
using GitDevelop.Domain.Accounts.Commands;

namespace GitDevelop.Application.Accounts.CommandHandlers;

public sealed class RegistrationCommandHandler : IRegistrationCommandHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IRequestContext _requestContext;
    private readonly ITokenProvider _tokenProvider;

    public RegistrationCommandHandler(
        IAccountRepository accountRepository,
        IRequestContext requestContext,
        ITokenProvider tokenProvider)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(tokenProvider);

        _accountRepository = accountRepository;
        _requestContext = requestContext;
        _tokenProvider = tokenProvider;
    }

    public async Task<AccountResponse> HandleAsync(RegistrationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var account = await _accountRepository
                          .FindByIdAsync(_requestContext.AccountId, cancellationToken)
                          .ConfigureAwait(false)
                      ?? Account.Create(request.Name, request.Email, request.Password);

        await _accountRepository.SetAsync(account, cancellationToken)
            .ConfigureAwait(false);

        var token = _tokenProvider.CreateToken(account); 

        return AccountResponse.Create(account, token);
    }
}
