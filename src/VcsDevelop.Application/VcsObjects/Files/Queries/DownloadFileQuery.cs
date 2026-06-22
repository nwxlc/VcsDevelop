namespace VcsDevelop.Application.VcsObjects.Files.Queries;

public sealed class DownloadFileQuery
{
    public Guid RepoId { get; private init; }
    public string? Path { get; private init; }

    private DownloadFileQuery(Guid repoId, string path)
    {
        RepoId = repoId;
        Path = path;
    }

    public static DownloadFileQuery Create(Guid repoId, string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        return new DownloadFileQuery(repoId, path);
    }
}
