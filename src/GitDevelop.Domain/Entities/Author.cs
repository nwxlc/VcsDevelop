namespace GitDevelop.Domain.Entities;

public sealed class Author
{
    public Guid Id { get; private init; }
    public string Email { get; private init; }
    public string Name { get; private init; }

    public Author(
        Guid id,
        string email,
        string name)
    {
        Id = id;
        Email = email;
        Name = name;
    }
}
