namespace GitDevelop.Domain.Entities;

public class Tree
{
    public Guid Id { get; set; }
    public IReadOnlyCollection<TreeEntry> Entries { get; set; }
    public ContentHash Hash { get; set; }
}
