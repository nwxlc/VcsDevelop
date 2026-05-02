namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class UploadedFileReference
{
    public Guid UploadId { get; init; }
    public Guid AccountId { get; init; }
    public string BlobId { get; init; }
    public string FileName { get; init; }
    public string ObjectKey { get; init; }
    public long Size { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public static TimeSpan DefaultLifetime => TimeSpan.FromHours(24);

    public UploadedFileReference(
        Guid uploadId,
        Guid accountId,
        string blobId,
        string fileName,
        string objectKey,
        long size,
        DateTime createdAt,
        DateTime expiresAt)
    {
        UploadId = uploadId;
        AccountId = accountId;
        BlobId = blobId;
        FileName = fileName;
        ObjectKey = objectKey;
        Size = size;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static UploadedFileReference Create(
        Guid accountId,
        string blobId,
        string fileName,
        string objectKey,
        long size)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobId);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);
        ArgumentOutOfRangeException.ThrowIfNegative(size);

        var createdAt = DateTime.UtcNow;

        return new UploadedFileReference(
            Guid.NewGuid(),
            accountId,
            blobId,
            fileName,
            objectKey,
            size,
            createdAt,
            createdAt.Add(DefaultLifetime));
    }
}
