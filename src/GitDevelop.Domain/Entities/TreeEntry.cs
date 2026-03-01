namespace GitDevelop.Domain.Entities;

public sealed class TreeEntry
{
    public string Name { get; private init; }
    public string ObjectId { get; private init; }

    public TreeEntry(string name, string objectId)
    {
        Name = name;
        ObjectId = objectId;
    }
}
