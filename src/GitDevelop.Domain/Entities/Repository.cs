namespace GitDevelop.Domain.Entities;

public sealed class Repository
{
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string DefaultBrancName { get; private init; }
    public RepositoryMetadata Metadata { get; private init; }
    public DateTime CreatedAt { get; private init; }

    public Repository(
        Guid id,
        string name,
        string defaultBrancName,
        RepositoryMetadata metadata,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        DefaultBrancName = defaultBrancName;
        Metadata = metadata;
        CreatedAt = createdAt;
    }
}
