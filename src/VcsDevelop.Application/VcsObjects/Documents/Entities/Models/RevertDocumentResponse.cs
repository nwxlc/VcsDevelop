namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class RevertDocumentResponse
{
    public Guid DocumentId { get; init; }
    public string BranchName { get; init; }
    public string RevertedToCommitId { get; init; } = null!;
    public string NewCommitId { get; init; } = null!;
    public string TreeId { get; init; } = null!;
}
