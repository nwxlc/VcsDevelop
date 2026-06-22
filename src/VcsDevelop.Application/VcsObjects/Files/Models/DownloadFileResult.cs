namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class DownloadFileResult : IAsyncDisposable
{
    public Stream Stream { get; init; }
    public string ContentType { get; init; }
    public string FileName { get; init; }
    public long Length { get; init; }
    
    public DownloadFileResult(
        Stream stream,
        string contentType,
        string fileName,
        long length)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Stream = stream;
        ContentType = contentType;
        FileName = fileName;
        Length = length;
    }
    
    private bool _disposed;
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await Stream.DisposeAsync().ConfigureAwait(false);
    }
}
