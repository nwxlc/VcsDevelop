namespace VcsDevelop.WebApi.Contracts.VcsObjects;

public sealed class CreateDocumentRequest
{
    public required string Name { get; init; }
    public string? DefaultBranchName { get; init; }
    public string? Description { get; init; }
    public List<string>? Tags { get; init; }
}
