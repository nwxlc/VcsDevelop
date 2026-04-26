namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public sealed class DocumentResponse
{
    public string Name { get; init; } = null!;
    public string DefaultBranchName { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public IReadOnlyCollection<string>? Tags { get; init; }
}
