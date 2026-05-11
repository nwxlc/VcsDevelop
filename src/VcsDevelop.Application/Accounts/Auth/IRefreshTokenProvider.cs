using VcsDevelop.Domain.Accounts;

namespace VcsDevelop.Application.Accounts.Auth;

public interface IRefreshTokenProvider
{
    Task<Token> CreateRefreshTokenAsync(Account account, CancellationToken cancellationToken);
    Task<Guid> ValidateAndRevokeRefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string tokenValue, CancellationToken cancellationToken);
    Task RevokeAllTokensAsync(Guid accountId, CancellationToken cancellationToken);
}
