namespace VcsDevelop.WebApi.Contracts.VcsObjects;

public sealed class RevertDocumentRequest
{
    public string CommitId { get; init; } = null!;
    public string BranchName { get; init; } = null!;
}
