namespace VcsDevelop.WebApi.Contracts.VcsObjects;

public sealed class StageDocumentFileRequest
{
    public required Guid UploadId { get; init; }
    public string? RepositoryPath { get; init; }
}
