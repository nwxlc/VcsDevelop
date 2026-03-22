namespace GitDevelop.Domain.Entities;

public sealed class Tree
{
    public Guid Id { get; private init; }
    public IReadOnlyCollection<TreeEntry> Entries { get; private init; } = Array.Empty<TreeEntry>();
    public ContentHash Hash { get; private init; } = null!;

    //ED only
    private Tree()
    {
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
