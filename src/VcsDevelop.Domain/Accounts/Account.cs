namespace VcsDevelop.Domain.Accounts;

public sealed class Account
{
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string Email { get; private init; }
    public Password Password { get; private init; }
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public bool IsActive { get; private set; }

    // EF only
    private Account()
    {
        Password = null!;
    }

    private Account(
        Guid id,
        string name,
        string email,
        Password password,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
        IsActive = true;
    }

    public static Account Create(string name, string email, Password password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(password);

        return new Account(Guid.NewGuid(), name, email, password, DateTime.UtcNow);
    }

    public void UpdateProfile(string? bio, string? avatarUrl)
    {
        Bio = bio;
        AvatarUrl = avatarUrl;
    }

    public void CheckPassword(Password password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (!Password.Verify(password))
        {
            throw new ArgumentException("Password is incorrect");
        }
    }
}
