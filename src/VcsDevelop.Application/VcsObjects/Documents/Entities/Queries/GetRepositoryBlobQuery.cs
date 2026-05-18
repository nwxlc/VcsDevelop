namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;

public sealed class GetRepositoryBlobQuery
{
    public Guid DocumentId { get; private init; }
    public string Path { get; private init; }

    private GetRepositoryBlobQuery(
        Guid documentId,
        string path)
    {
        DocumentId = documentId;
        Path = path;
    }

    public static GetRepositoryBlobQuery Create(
        Guid documentId,
        string path)
    {
        return new GetRepositoryBlobQuery(documentId, path);
    }
}
