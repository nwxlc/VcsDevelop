namespace GitDevelop.Domain.Entities;

public class Branch
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Name { get; set; }
    public Guid HeadCommitId { get; set; }
    public DateTime CreatedAt { get; set; }
}
