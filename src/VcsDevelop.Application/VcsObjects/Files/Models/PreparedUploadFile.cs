namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class PreparedUploadFile : IAsyncDisposable
{
    public string BlobId { get; init; }
    public string FileName { get; init; }
    public string TempFilePath { get; init; }
    public string ContentType { get; init; }
    public long Size { get; init; }

    private PreparedUploadFile(
        string blobId,
        string fileName,
        string tempFilePath,
        string contentType,
        long size)
    {
        BlobId = blobId;
        FileName = fileName;
        TempFilePath = tempFilePath;
        ContentType = contentType;
        Size = size;
    }

    public static PreparedUploadFile Create(
        string blobId,
        string fileName,
        string tempFilePath,
        string contentType,
        long size)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobId);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(tempFilePath);
        ArgumentOutOfRangeException.ThrowIfNegative(size);

        return new PreparedUploadFile(blobId, fileName, tempFilePath, contentType, size);
    }

    public ValueTask DisposeAsync()
    {
        if (File.Exists(TempFilePath))
        {
            File.Delete(TempFilePath);
        }

        return ValueTask.CompletedTask;
    }
}
