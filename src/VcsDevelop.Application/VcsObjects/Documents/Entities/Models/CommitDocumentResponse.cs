namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class CommitDocumentResponse
{
    public required Guid DocumentId { get; init; }
    public required string BranchName { get; init; }
    public required string CommitId { get; init; }
    public required string TreeId { get; init; }
    public required int FilesCommitted { get; init; }
}
