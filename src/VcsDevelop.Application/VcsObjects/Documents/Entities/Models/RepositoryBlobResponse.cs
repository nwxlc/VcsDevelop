namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class RepositoryBlobResponse
{
    public required Guid DocumentId { get; init; }
    public required string BranchName { get; init; }
    public required string Path { get; init; }
    public required string BlobId { get; init; }
    public required long Size { get; init; }
    public required string Content { get; init; }
}
