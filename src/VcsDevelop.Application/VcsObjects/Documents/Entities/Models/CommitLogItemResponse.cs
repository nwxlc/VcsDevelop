namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class CommitLogItemResponse
{
    public string Id { get; init; } = null!;
    public string TreeId { get; init; } = null!;
    public string Message { get; init; } = null!;
    public Guid AccountId { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<string> ParentIds { get; init; } = [];
}
