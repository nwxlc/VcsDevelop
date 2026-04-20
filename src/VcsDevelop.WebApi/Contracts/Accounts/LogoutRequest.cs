namespace VcsDevelop.WebApi.Contracts.Accounts;

public class LogoutRequest
{
    public string RefreshToken { get; init; } = null!;
}
