namespace VcsDevelop.Domain.VcsObjects.Commands;

public sealed class CommitDocumentCommand
{
    public Guid DocumentId { get; private init; }
    public string Message { get; private init; }

    private CommitDocumentCommand(
        Guid documentId,
        string message)
    {
        DocumentId = documentId;
        Message = message;
    }

    public static CommitDocumentCommand Create(
        Guid documentId,
        string message)
    {
        return new CommitDocumentCommand(documentId, message);
    }
}
