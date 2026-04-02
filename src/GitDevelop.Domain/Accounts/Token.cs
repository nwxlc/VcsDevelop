namespace GitDevelop.Domain.Accounts;

public sealed class Token
{
    public string Value { get; private init; }
    public DateTime? ExpirationDate { get; private init; }

    private Token(
        string value,
        DateTime? expirationDate)
    {
        Value = value;
        ExpirationDate = expirationDate;
    }

    public static Token Create(string value, DateTime? expirationDate)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        return new Token(value, expirationDate);
    }

    internal bool IsActive()
    {
        return ExpirationDate is null || ExpirationDate > DateTime.UtcNow;
    }
}
