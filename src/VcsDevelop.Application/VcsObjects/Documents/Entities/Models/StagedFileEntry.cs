namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class StagedFileEntry
{
    public required Guid DocumentId { get; init; }
    public required Guid AccountId { get; init; }
    public required Guid UploadId { get; init; }
    public required string BlobId { get; init; }
    public required string FileName { get; init; }
    public required string RepositoryPath { get; init; }
    public required string ObjectKey { get; init; }
    public required DateTime StagedAt { get; init; }
}
