namespace VcsDevelop.WebApi.Contracts.Accounts;

public sealed class LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
