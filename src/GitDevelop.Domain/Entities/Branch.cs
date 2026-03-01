namespace GitDevelop.Domain.Entities;

public sealed class Branch
{
    public Guid Id { get; private init; }
    public Guid DocumentId { get; private init; }
    public string Name { get; private init; }
    public Guid HeadCommitId { get; private init; }
    public DateTime CreatedAt { get; private init; }

    public Branch(
        Guid id,
        Guid documentId,
        string name,
        Guid headCommitId,
        DateTime createdAt)
    {
        Id = id;
        DocumentId = documentId;
        Name = name;
        HeadCommitId = headCommitId;
        CreatedAt = createdAt;
    }
}
