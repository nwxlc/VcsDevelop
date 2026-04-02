namespace GitDevelop.Domain.GitObjects;

public sealed class CommitParent
{
    public Guid ParentId { get; private init; }

    // EF only
    private CommitParent()
    {
    }

    public CommitParent(Guid parentId)
    {
        ParentId = parentId;
    }
}
