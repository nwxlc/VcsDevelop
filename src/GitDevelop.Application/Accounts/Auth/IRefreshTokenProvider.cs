using GitDevelop.Domain.Accounts;

namespace GitDevelop.Application.Accounts.Auth;

public interface IRefreshTokenProvider
{
    Task<Token> CreateRefreshTokenAsync(Account account, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string tokenValue, CancellationToken cancellationToken);
    Task RevokeAllTokensAsync(Guid accountId, CancellationToken cancellationToken);
}
