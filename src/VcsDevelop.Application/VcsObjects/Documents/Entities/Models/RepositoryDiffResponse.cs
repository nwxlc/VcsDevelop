namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class RepositoryDiffResponse
{
    public required Guid DocumentId { get; init; }
    public required string FromCommitId { get; init; }
    public required string ToCommitId { get; init; }
    public string? Path { get; init; }
    public required IReadOnlyCollection<RepositoryDiffEntryResponse> Entries { get; init; }
}
