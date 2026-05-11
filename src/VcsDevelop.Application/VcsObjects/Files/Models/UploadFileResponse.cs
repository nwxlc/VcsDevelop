namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class UploadFileResponse
{
    public Guid UploadId { get; init; }
    public string BlobId { get; init; }
    public string FileName { get; init; }
    public string ObjectKey { get; init; }
    public long Size { get; init; }
    public DateTime ExpiresAt { get; init; }

    private UploadFileResponse(
        Guid uploadId,
        string blobId,
        string fileName,
        string objectKey,
        long size,
        DateTime expiresAt)
    {
        UploadId = uploadId;
        BlobId = blobId;
        FileName = fileName;
        ObjectKey = objectKey;
        Size = size;
        ExpiresAt = expiresAt;
    }

    public static UploadFileResponse Create(
        Guid uploadId,
        string blobId,
        string fileName,
        string objectKey,
        long size,
        DateTime expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobId);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);
        ArgumentOutOfRangeException.ThrowIfNegative(size);

        return new UploadFileResponse(
            uploadId,
            blobId,
            fileName,
            objectKey,
            size,
            expiresAt);
    }
}
