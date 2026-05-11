using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Application.Accounts.Entities.Models;
using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Domain.Accounts.Commands;

namespace VcsDevelop.Application.Accounts.CommandHandlers;

public sealed class RefreshAccessTokenCommandHandler : IRefreshAccessTokenCommandHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly ITokenProvider _tokenProvider;

    public RefreshAccessTokenCommandHandler(
        IAccountRepository accountRepository,
        IRefreshTokenProvider refreshTokenProvider,
        ITokenProvider tokenProvider)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(refreshTokenProvider);
        ArgumentNullException.ThrowIfNull(tokenProvider);

        _accountRepository = accountRepository;
        _refreshTokenProvider = refreshTokenProvider;
        _tokenProvider = tokenProvider;
    }

    public async Task<AccountResponse> HandleAsync(
        RefreshAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = await _refreshTokenProvider
            .ValidateAndRevokeRefreshTokenAsync(request.RefreshToken, cancellationToken)
            .ConfigureAwait(false);

        var account = await _accountRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);

        var token = _tokenProvider.CreateToken(account);

        var refreshToken = await _refreshTokenProvider.CreateRefreshTokenAsync(account, cancellationToken);

        return AccountResponse.Create(account, token, refreshToken);
    }
}
