namespace GitDevelop.Domain.Entities;

public sealed class Branch
{
    public Guid Id { get; private init; }
    public Guid RepositoryId { get; private init; }
    public string Name { get; private init; }
    public Guid HeadCommitId { get; private init; }
    public DateTime CreatedAt { get; private init; }

    public Branch(
        Guid id,
        Guid repositoryId,
        string name,
        Guid headCommitId,
        DateTime createdAt)
    {
        Id = id;
        RepositoryId = repositoryId;
        Name = name;
        HeadCommitId = headCommitId;
        CreatedAt = createdAt;
    }
}
