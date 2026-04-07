using GitDevelop.Domain.Accounts;

namespace GitDevelop.Application.Accounts.Entities;

public sealed class AccountResponse
{
    public Guid AccountId { get; }
    public Token AccessToken { get; }
    public Token RefreshToken { get; }

    private AccountResponse(
        Guid accountId,
        Token accessToken,
        Token refreshToken)
    {
        AccountId = accountId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public static AccountResponse Create(Account account, Token accessToken, Token refreshToken)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(accessToken);
        ArgumentNullException.ThrowIfNull(refreshToken);

        return new AccountResponse(account.Id, accessToken, refreshToken);
    }
}
