namespace VcsDevelop.Domain.VcsObjects.Commands;

public sealed class StageDocumentFileCommand
{
    public Guid DocumentId { get; private init; }
    public Guid UploadId { get; private init; }
    public string? RepositoryPath { get; private init; }

    private StageDocumentFileCommand(
        Guid documentId,
        Guid uploadId,
        string? repositoryPath = null)
    {
        DocumentId = documentId;
        UploadId = uploadId;
        RepositoryPath = repositoryPath;
    }

    public static StageDocumentFileCommand Create(
        Guid documentId,
        Guid uploadId,
        string? repositoryPath = null)
    {
        return new StageDocumentFileCommand(documentId, uploadId, repositoryPath);
    }
}
