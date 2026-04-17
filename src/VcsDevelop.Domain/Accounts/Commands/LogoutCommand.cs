namespace VcsDevelop.Domain.Accounts.Commands;

public sealed class LogoutCommand
{
    public string RefreshToken { get; }

    private LogoutCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public static LogoutCommand Create(string refreshToken)
        => new(refreshToken);
}
