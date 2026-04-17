using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Domain.Accounts.Commands;

namespace VcsDevelop.Application.Accounts.CommandHandlers;

public sealed class LogoutCommandHandler : ILogoutCommandHandler
{
    private readonly IRefreshTokenProvider _refreshTokenProvider;

    public LogoutCommandHandler(IRefreshTokenProvider refreshTokenProvider)
    {
        ArgumentNullException.ThrowIfNull(refreshTokenProvider);

        _refreshTokenProvider = refreshTokenProvider;
    }

    public async Task HandleAsync(LogoutCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        await _refreshTokenProvider.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken)
            .ConfigureAwait(false);
    }
}
