namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class DocumentLogResponse
{
    public Guid DocumentId { get; init; }
    public string BranchName { get; init; } = null!;
    public IReadOnlyCollection<CommitLogItemResponse> Commits { get; init; } = [];
}
