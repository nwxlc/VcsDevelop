namespace VcsDevelop.WebApi.Contracts.VcsObjects;

public sealed class CommitDocumentRequest
{
    public required string Message { get; init; }
}
