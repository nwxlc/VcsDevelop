namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class RepositoryTreeEntryResponse
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required string Type { get; init; }
    public string? BlobId { get; init; }
}
