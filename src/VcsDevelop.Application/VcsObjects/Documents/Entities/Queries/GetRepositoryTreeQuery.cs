namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;

public sealed class GetRepositoryTreeQuery
{
    public Guid DocumentId { get; private init; }
    public string? Path { get; private init; }

    private GetRepositoryTreeQuery(
        Guid documentId,
        string? path)
    {
        DocumentId = documentId;
        Path = path;
    }

    public static GetRepositoryTreeQuery Create(
        Guid documentId,
        string? path)
    {
        return new GetRepositoryTreeQuery(documentId, path);
    }
}
