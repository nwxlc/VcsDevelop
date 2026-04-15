namespace VcsDevelop.Domain.VcsObjects;

public sealed class Commit
{
    public string Id { get; private init; }
    public Guid DocumentId { get; private init; }
    public string RootTreeId { get; private init; }
    public IReadOnlyCollection<CommitParent> ParentIds { get; private init; }
    public Guid AccountId { get; private init; }
    public CommitMessage Message { get; private init; }
    public DateTime CreatedAt { get; private init; }

    // EF only
    private Commit()
    {
        ParentIds = new HashSet<CommitParent>();
    }

    public Commit(
        string id,
        Guid documentId,
        string rootTreeId,
        IReadOnlyCollection<string> parentIds,
        Guid accountId,
        CommitMessage message,
        DateTime createdAt)
    {
        Id = id;
        DocumentId = documentId;
        RootTreeId = rootTreeId;
        ParentIds = parentIds.Select(parentId => new CommitParent(parentId)).ToArray();
        AccountId = accountId;
        Message = message;
        CreatedAt = createdAt;
    }
}
