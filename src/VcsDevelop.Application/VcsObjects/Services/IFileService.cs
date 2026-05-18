namespace VcsDevelop.Application.VcsObjects.Services;

public interface IFileService
{
    Task UploadFileAsync(
        Stream stream,
        string key,
        long length,
        CancellationToken cancellationToken);

    Task<Stream> DownloadFileAsync(
        string key,
        CancellationToken cancellationToken);

    Task DeleteFileAsync(
        string key,
        CancellationToken cancellationToken);
}
