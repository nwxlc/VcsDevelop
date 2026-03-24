namespace GitDevelop.Domain.Entities;

public sealed class Document
{
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string DefaultBranchName { get; private init; }
    public DocumentMetadata Metadata { get; private init; }
    public DateTime CreatedAt { get; private init; }

    public Document(
        Guid id,
        string name,
        string defaultBranchName,
        DocumentMetadata metadata,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        DefaultBranchName = defaultBranchName;
        Metadata = metadata;
        CreatedAt = createdAt;
    }
}
