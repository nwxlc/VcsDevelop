namespace VcsDevelop.Domain.VcsObjects;

public sealed class Document
{
    public Guid Id { get; private init; }
    public Guid OwnerId { get; private init; }
    public string Name { get; private init; }
    public string DefaultBranchName { get; private init; }
    public DocumentMetadata Metadata { get; private init; }
    public DateTime CreatedAt { get; private init; }

    // EF only
    private Document()
    {
        Metadata = null!;
    }

    private Document(
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

    public static Document Create(
        string name,
        string defaultBranchName,
        DocumentMetadata metadata)
    {
        var createdAt = DateTime.UtcNow;

        return new Document(
            Guid.NewGuid(),
            name,
            defaultBranchName,
            metadata,
            createdAt);
    }
}
