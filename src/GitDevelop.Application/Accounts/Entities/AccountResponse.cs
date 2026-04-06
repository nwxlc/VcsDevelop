using GitDevelop.Domain.Accounts;

namespace GitDevelop.Application.Accounts.Entities;

public sealed class AccountResponse
{
    public Guid AccountId { get; }
    public Token AccessToken { get; }

    private AccountResponse(
        Guid accountId,
        Token token)
    {
        AccountId = accountId;
        AccessToken = token;
    }

    public static AccountResponse Create(Account account, Token token)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(token);

        return new AccountResponse(account.Id, token);
    }
}
