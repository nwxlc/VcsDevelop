namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models.Paging;

public sealed class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public PaginationMetadata Metadata { get; set; } = null;
}
