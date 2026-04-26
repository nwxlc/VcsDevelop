namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;

public sealed class GetDocumentByIdQuery
{
    public Guid Id { get; }

    private GetDocumentByIdQuery(Guid id)
    {
        Id = id;
    }

    public static GetDocumentByIdQuery Create(Guid id)
    {
        return new GetDocumentByIdQuery(id);
    }
}
