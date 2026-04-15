namespace VcsDevelop.Domain.Accounts.Commands;

public sealed class LoginCommand
{
    public string Email { get; private init; }
    public Password Password { get; private init; }

    private LoginCommand(
        string email,
        Password password)
    {
        Email = email;
        Password = password;
    }

    public static LoginCommand Create(string email, Password password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(password);

        return new LoginCommand(email, password);
    }
}
