namespace VcsDevelop.WebApi.Contracts;

public class LogoutRequest
{
    public string RefreshToken { get; init; } = null!;
}
