namespace VcsDevelop.Application.VcsObjects.Models;

public sealed class PreparedUploadFile : IAsyncDisposable
{
    public string BlobId { get; init; }
    public string FileName { get; init; }
    public string TempFilePath { get; init; }
    public long Size { get; init; }

    private PreparedUploadFile(
        string blobId,
        string fileName,
        string tempFilePath,
        long size)
    {
        BlobId = blobId;
        FileName = fileName;
        TempFilePath = tempFilePath;
        Size = size;
    }

    public static PreparedUploadFile Create(string blobId, string fileName, string tempFilePath, long size)
    {
        return new PreparedUploadFile(blobId, fileName, tempFilePath, size);
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
