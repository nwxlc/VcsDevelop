namespace VcsDevelop.Domain.VcsObjects.Commands;

public sealed class RevertDocumentCommand
{
    public Guid DocumentId { get; private init; }
    public string CommitId { get; private init; }
    public string BranchName { get; private init; }

    private RevertDocumentCommand(
        Guid documentId,
        string commitId,
        string branchName)
    {
        DocumentId = documentId;
        CommitId = commitId;
        BranchName = branchName;
    }

    public static RevertDocumentCommand Create(
        Guid documentId,
        string commitId,
        string branchName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);
        ArgumentException.ThrowIfNullOrWhiteSpace(branchName);

        return new RevertDocumentCommand(documentId, commitId, branchName);
    }
}
