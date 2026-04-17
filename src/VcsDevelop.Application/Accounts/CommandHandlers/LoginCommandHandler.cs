using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Application.Accounts.Entities;
using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.Accounts.Commands;

namespace VcsDevelop.Application.Accounts.CommandHandlers;

public sealed class LoginCommandHandler : ILoginCommandHandler
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly IAccountRepository _accountRepository;
    private readonly IRequestContext _requestContext;

    public LoginCommandHandler(
        ITokenProvider tokenProvider,
        IRefreshTokenProvider refreshTokenProvider,
        IAccountRepository accountRepository,
        IRequestContext requestContext)
    {
        ArgumentNullException.ThrowIfNull(tokenProvider);
        ArgumentNullException.ThrowIfNull(refreshTokenProvider);
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(requestContext);

        _tokenProvider = tokenProvider;
        _refreshTokenProvider = refreshTokenProvider;
        _accountRepository = accountRepository;
        _requestContext = requestContext;
    }

    public async Task<AccountResponse> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var account = await _accountRepository
            .GetByEmailAsync(request.Email, cancellationToken)
            .ConfigureAwait(false);

        account.CheckPassword(request.Password);

        var token = _tokenProvider.CreateToken(account);

        var refreshToken = await _refreshTokenProvider.CreateRefreshTokenAsync(account, cancellationToken);

        return AccountResponse.Create(account, token, refreshToken);
    }
}
