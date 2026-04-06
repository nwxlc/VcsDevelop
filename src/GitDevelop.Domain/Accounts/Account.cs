namespace GitDevelop.Domain.Accounts;

public sealed class Account
{
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string Email { get; private init; }
    public Password Password { get; private init; }

    // EF only
    private Account()
    {
        Password = null!;
    }

    private Account(
        Guid id,
        string name,
        string email,
        Password password)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
    }

    public static Account Create(string name, string email, Password password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(password);

        return new Account(Guid.NewGuid(), name, email, password);
    }
}
