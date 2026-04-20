namespace VcsDevelop.Application.Accounts.Entities.Models;

public sealed class AccountInfoResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Bio { get; init; }
    public string? AvatarUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}
