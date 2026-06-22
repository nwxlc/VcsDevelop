using VcsDevelop.Application.VcsObjects.Files.Models;

namespace VcsDevelop.Application.VcsObjects.Services;

public interface IFileService
{
    Task UploadFileAsync(
        Stream stream,
        string key,
        long length,
        string? contentType,
        CancellationToken cancellationToken);

    Task<DownloadFileResult> DownloadFileAsync(
        string key,
        CancellationToken cancellationToken);

    Task DeleteFileAsync(
        string key,
        CancellationToken cancellationToken);
}
