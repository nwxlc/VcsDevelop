namespace VcsDevelop.Domain.VcsObjects;

public class Blob
{
    public string Id { get; private init; }
    public long Size { get; private init; }
    public DateTime CreatedAt { get; private init; }

    private Blob(
        string id,
        long size,
        DateTime createdAt)
    {
        Id = id;
        Size = size;
        CreatedAt = createdAt;
    }

    public static Blob Create(string id, long size)
    {
        return new Blob(id, size, DateTime.UtcNow);
    }
}
