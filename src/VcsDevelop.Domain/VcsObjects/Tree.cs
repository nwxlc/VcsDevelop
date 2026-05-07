namespace VcsDevelop.Domain.VcsObjects;

public sealed class Tree
{
    public string Id { get; private init; }
    public IReadOnlyCollection<TreeEntry> Entries { get; private set; }

    // EF only
    private Tree()
    {
        Id = null!;
        Entries = new HashSet<TreeEntry>();
    }

    public Tree(
        string id,
        IReadOnlyCollection<TreeEntry> entries)
    {
        Id = id;
        Entries = entries;
    }
}
