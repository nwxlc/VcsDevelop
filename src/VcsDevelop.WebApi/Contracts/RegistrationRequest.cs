namespace VcsDevelop.WebApi.Contracts;

public sealed class RegistrationRequest
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
