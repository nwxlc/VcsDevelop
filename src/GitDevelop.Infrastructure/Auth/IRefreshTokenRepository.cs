namespace GitDevelop.Infrastructure.Auth;

public interface IRefreshTokenRepository
{
    Task SetAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken);
    Task RemoveAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task RemoveAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
}
