namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;

public sealed class GetRepositoryDiffQuery
{
    public Guid DocumentId { get; private init; }
    public string FromCommitId { get; private init; }
    public string ToCommitId { get; private init; }
    public string? Path { get; private init; }

    private GetRepositoryDiffQuery(
        Guid documentId,
        string fromCommitId,
        string toCommitId,
        string? path)
    {
        DocumentId = documentId;
        FromCommitId = fromCommitId;
        ToCommitId = toCommitId;
        Path = path;
    }

    public static GetRepositoryDiffQuery Create(
        Guid documentId,
        string fromCommitId,
        string toCommitId,
        string? path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fromCommitId);
        ArgumentException.ThrowIfNullOrWhiteSpace(toCommitId);

        return new GetRepositoryDiffQuery(documentId, fromCommitId, toCommitId, path);
    }
}
