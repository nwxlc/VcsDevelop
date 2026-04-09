namespace VcsDevelop.Domain.VcsObjects;

public sealed class Tree
{
    public Guid Id { get; private init; }
    public IReadOnlyCollection<TreeEntry> Entries { get; private set; }
    public ContentHash Hash { get; private init; }

    // EF only
    private Tree()
    {
        Entries = new HashSet<TreeEntry>();
        Hash = null!;
    }

    public Tree(
        Guid id,
        IReadOnlyCollection<TreeEntry> entries,
        ContentHash hash)
    {
        Id = id;
        Entries = entries;
        Hash = hash;
    }
}
