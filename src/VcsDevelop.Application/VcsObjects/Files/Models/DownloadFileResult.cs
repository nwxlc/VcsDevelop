namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class DownloadFileResult : IAsyncDisposable
{
    public Stream Stream { get; init; }
    public string ContentType { get; init; }
    public long Length { get; init; }
    
    public DownloadFileResult(
        Stream stream,
        string contentType,
        long length)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Stream = stream;
        ContentType = contentType;
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
