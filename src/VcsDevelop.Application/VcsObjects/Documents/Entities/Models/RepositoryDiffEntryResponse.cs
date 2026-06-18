namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class RepositoryDiffEntryResponse
{
    public required string Path { get; init; }
    public required string ChangeType { get; init; }
    public string? OldBlobId { get; init; }
    public string? NewBlobId { get; init; }
}
