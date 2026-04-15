namespace VcsDevelop.Domain.VcsObjects;

public sealed class CommitParent
{
    public string ParentId { get; private init; }

    // EF only
    private CommitParent()
    {
    }

    public CommitParent(string parentId)
    {
        ParentId = parentId;
    }
}
