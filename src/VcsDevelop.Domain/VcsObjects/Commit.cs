namespace VcsDevelop.Domain.VcsObjects;

public sealed class Commit
{
    public Guid Id { get; private init; }
    public Guid DocumentId { get; private init; }
    public Guid RootTreeId { get; private init; }
    public IReadOnlyCollection<CommitParent> ParentIds { get; private init; }
    public Guid AccountId { get; private init; }
    public CommitMessage Message { get; private init; }
    public DateTime CreatedAt { get; private init; }
    public ContentHash Hash { get; private init; }

    // EF only
    private Commit()
    {
        Hash = null!;
        ParentIds = new HashSet<CommitParent>();
    }

    public Commit(
        Guid id,
        Guid documentId,
        Guid rootTreeId,
        IReadOnlyCollection<Guid> parentIds,
        Guid accountId,
        CommitMessage message,
        DateTime createdAt,
        ContentHash hash)
    {
        Id = id;
        DocumentId = documentId;
        RootTreeId = rootTreeId;
        ParentIds = parentIds.Select(parentId => new CommitParent(parentId)).ToArray();
        AccountId = accountId;
        Message = message;
        CreatedAt = createdAt;
        Hash = hash;
    }
}
