namespace VcsDevelop.WebApi.Contracts.Accounts;

public sealed class UpdateAccountRequest
{
    public string Name { get; init; } = null!;
    public string? Bio { get; init; }
    public string? AvatarUrl { get; init; }
}
