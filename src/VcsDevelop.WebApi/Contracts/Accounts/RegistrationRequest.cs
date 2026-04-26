namespace VcsDevelop.WebApi.Contracts.Accounts;

public sealed class RegistrationRequest
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
