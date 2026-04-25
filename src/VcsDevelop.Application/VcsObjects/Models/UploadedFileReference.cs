namespace VcsDevelop.Application.VcsObjects.Models;

public sealed class UploadedFileReference
{
    public Guid UploadId { get; init; }
    public Guid AccountId { get; init; }
    public string BlobId { get; init; }
    public string FileName { get; init; }
    public string ObjectKey { get; init; }
    public long Size { get; init; }
    public DateTime CreatedAt { get; init; }

    private UploadedFileReference(
        Guid uploadId,
        Guid accountId,
        string blobId,
        string fileName,
        string objectKey,
        long size,
        DateTime createdAt)
    {
        UploadId = uploadId;
        AccountId = accountId;
        BlobId = blobId;
        FileName = fileName;
        ObjectKey = objectKey;
        Size = size;
        CreatedAt = createdAt;
    }

    public static UploadedFileReference Create(
        Guid accountId,
        string blobId,
        string fileName,
        string objectKey,
        long size)
    {
        return new UploadedFileReference(
            Guid.NewGuid(),
            accountId,
            blobId,
            fileName,
            objectKey,
            size,
            DateTime.UtcNow);
    }
}
