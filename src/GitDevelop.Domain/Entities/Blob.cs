namespace GitDevelop.Domain.Entities;

public class Blob
{
    public Guid Id { get; private init; }
    public ContentHash Hash { get; private init; }
    public long Size { get; private init; }
    public DateTime CreatedAt { get; private init; }

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
