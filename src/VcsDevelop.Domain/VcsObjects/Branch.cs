namespace VcsDevelop.Domain.VcsObjects;

public sealed class Branch
{
    public Guid Id { get; private init; }
    public Guid DocumentId { get; private init; }
    public string Name { get; private init; }
    public string HeadCommitId { get; private set; }
    public DateTime CreatedAt { get; private init; }

    public Branch(
        Guid id,
        Guid documentId,
        string name,
        string headCommitId,
        DateTime createdAt)
    {
        Id = id;
        DocumentId = documentId;
        Name = name;
        HeadCommitId = headCommitId;
        CreatedAt = createdAt;
    }

    public static Branch Create(Guid documentId, string name, string headCommitId)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(headCommitId);
        
        var createdAt = DateTime.UtcNow;
        
        return new Branch(
            Guid.NewGuid(),
            documentId,
            name,
            headCommitId,
            createdAt);
    }

    public void UpdateHeadCommit(string headCommitId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headCommitId);

        HeadCommitId = headCommitId;
    }
}
