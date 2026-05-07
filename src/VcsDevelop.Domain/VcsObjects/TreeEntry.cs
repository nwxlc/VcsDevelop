namespace VcsDevelop.Domain.VcsObjects;

public sealed class TreeEntry
{
    public string Name { get; private init; }
    public string ObjectId { get; private init; }

    // EF only
    private TreeEntry()
    {
        Name = null!;
        ObjectId = null!;
    }

    public TreeEntry(
        string name,
        string objectId)
    {
        Name = name;
        ObjectId = objectId;
    }
}
