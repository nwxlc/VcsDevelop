namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class RepositoryTreeResponse
{
    public required Guid DocumentId { get; init; }
    public required string BranchName { get; init; }
    public required string Path { get; init; }
    public required IReadOnlyCollection<RepositoryTreeEntryResponse> Entries { get; init; }
}
