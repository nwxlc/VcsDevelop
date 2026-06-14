namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;

public sealed class GetDocumentLogQuery
{
    public Guid DocumentId { get; private init; }

    private GetDocumentLogQuery(
        Guid documentId)
    {
        DocumentId = documentId;
    }

    public static GetDocumentLogQuery Create(Guid documentId)
    {
        return new GetDocumentLogQuery(documentId);
    }
}
