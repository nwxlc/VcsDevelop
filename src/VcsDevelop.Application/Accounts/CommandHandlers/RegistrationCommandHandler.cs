using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Application.Accounts.Entities.Models;
using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Domain.Accounts.Commands;

namespace VcsDevelop.Application.Accounts.CommandHandlers;

public sealed class RegistrationCommandHandler : IRegistrationCommandHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IRequestContext _requestContext;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;

    public RegistrationCommandHandler(
        IAccountRepository accountRepository,
        IRequestContext requestContext,
        ITokenProvider tokenProvider,
        IRefreshTokenProvider refreshTokenProvider)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(tokenProvider);
        ArgumentNullException.ThrowIfNull(refreshTokenProvider);

        _accountRepository = accountRepository;
        _requestContext = requestContext;
        _tokenProvider = tokenProvider;
        _refreshTokenProvider = refreshTokenProvider;
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

        var refreshToken = await _refreshTokenProvider.CreateRefreshTokenAsync(account, cancellationToken);

        return AccountResponse.Create(account, token, refreshToken);
    }
}
