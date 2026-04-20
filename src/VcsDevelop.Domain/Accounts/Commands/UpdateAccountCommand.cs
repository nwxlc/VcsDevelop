namespace VcsDevelop.Domain.Accounts.Commands;

public sealed class UpdateAccountCommand
{
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string? Bio { get; private init; }
    public string? AvatarUrl { get; private init; }

    private UpdateAccountCommand(
        Guid id,
        string name,
        string? bio,
        string? avatarUrl)
    {
        Name = name;
        Bio = bio;
        AvatarUrl = avatarUrl;
        Id = id;
    }

    public static UpdateAccountCommand Create(Guid id, string name, string? bio, string? avatarUrl)
    {
        return new UpdateAccountCommand(id, name, bio, avatarUrl);
    }
}
