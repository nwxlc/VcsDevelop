namespace GitDevelop.Domain.GitObjects;

public sealed class TreeEntry
{
    public string Name { get; private init; }
    public string ObjectId { get; private init; }

    // EF onlu
    private TreeEntry()
    {
    }

    public TreeEntry(
        string name,
        string objectId)
    {
        Name = name;
        ObjectId = objectId;
    }
}
