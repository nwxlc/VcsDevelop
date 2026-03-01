namespace GitDevelop.Domain.Entities;

public sealed class Commit
{
    public Guid Id { get; private init; }
    public Guid DocumentId { get; private init; }
    public Guid RootTreeId { get; private init; }
    public IReadOnlyCollection<Guid> ParentIds { get; private init; }
    public Author Author { get; private init; }
    public CommitMessage Message { get; private init; }
    public DateTime CreatedAt { get; private init; }
    public ContentHash Hash { get; private init; }

    public Commit(
        Guid id,
        Guid documentId,
        Guid rootTreeId,
        IReadOnlyCollection<Guid> parentIds,
        Author author,
        CommitMessage message,
        DateTime createdAt,
        ContentHash hash)
    {
        Id = id;
        DocumentId = documentId;
        RootTreeId = rootTreeId;
        ParentIds = parentIds;
        Author = author;
        Message = message;
        CreatedAt = createdAt;
        Hash = hash;
    }
}
