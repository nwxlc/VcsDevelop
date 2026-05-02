namespace VcsDevelop.Domain.Accounts.Commands;

public class RefreshAccessTokenCommand
{
    public string RefreshToken { get; set; }
    
    private RefreshAccessTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public static RefreshAccessTokenCommand Create(string refreshToken)
        => new(refreshToken);
}
