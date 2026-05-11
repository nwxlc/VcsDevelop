namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class StoredFileResult
{
    public required string BlobId { get; init; }
    public required string ObjectKey { get; init; }
    public required bool BlobCreated { get; init; }
    public required bool ObjectUploaded { get; init; }
}
