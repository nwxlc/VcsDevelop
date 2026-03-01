namespace GitDevelop.Domain.Entities;

public class Commit
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid RootTreeId { get; set; }
    public IReadOnlyCollection<Guid> ParentIds { get; set; }
    public Author Author { get; set; }
    public CommitMessage Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public ContentHash Hash { get; set; }
}
