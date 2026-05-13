namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;

public sealed class GetDocumentsQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    private GetDocumentsQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static GetDocumentsQuery Create(int pageNumber, int pageSize)
    {
        return new GetDocumentsQuery(pageNumber, pageSize);
    }
}
