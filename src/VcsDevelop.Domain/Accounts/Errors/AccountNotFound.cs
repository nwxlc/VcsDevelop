using VcsDevelop.Core.Errors;

namespace VcsDevelop.Domain.Accounts.Errors;

public class AccountNotFound : NotFound
{
    private AccountNotFound(Guid? accountId, string? email)
    {
        if (accountId == null && string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Either accountId or email must be provided", nameof(accountId));
        }

        AccountId = accountId;
        Email = email;
    }

    public static AccountNotFound ById(Guid accountId) => new(accountId, email: null);
    public static AccountNotFound ByEmail(string email) => new(accountId: null, email);

    public override string Message => "Account not found.";

    public Guid? AccountId { get; }
    public string? Email { get; }
}
