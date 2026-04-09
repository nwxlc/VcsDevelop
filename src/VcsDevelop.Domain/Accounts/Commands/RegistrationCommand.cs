namespace VcsDevelop.Domain.Accounts.Commands;

public sealed class RegistrationCommand
{
    public string Name { get; private init; }
    public string Email { get; private init; }
    public Password Password { get; private init; }

    private RegistrationCommand(
        string name,
        string email,
        Password password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public static RegistrationCommand Create(string name, string email, Password password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(password);

        return new RegistrationCommand(name, email, password);
    }
}
