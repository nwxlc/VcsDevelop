namespace GitDevelop.Domain.GitObjects;

public class Blob
{
    public Guid Id { get; private init; }
    public ContentHash Hash { get; private init; }
    public long Size { get; private init; }
    public DateTime CreatedAt { get; private init; }

    // EF Only
    private Blob()
    {
        Hash = null!;
    }

    public Blob(
        Guid id,
        ContentHash hash,
        long size,
        DateTime createdAt)
    {
        Id = id;
        Hash = hash;
        Size = size;
        CreatedAt = createdAt;
    }
}
