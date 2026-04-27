namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class CompressionResult : IAsyncDisposable
{
    private readonly string? _tempPath;

    public Stream Stream { get; }
    public long Length { get; }

    private CompressionResult(Stream stream, long length, string? tempPath)
    {
        Stream = stream;
        Length = length;
        _tempPath = tempPath;
    }

    public static CompressionResult Create(Stream stream, long length, string? tempPath = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return new CompressionResult(stream, length, tempPath);
    }

    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync().ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(_tempPath) && File.Exists(_tempPath))
        {
            File.Delete(_tempPath);
        }
    }
}
