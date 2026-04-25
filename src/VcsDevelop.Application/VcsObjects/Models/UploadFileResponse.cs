namespace VcsDevelop.Application.VcsObjects.Models;

public sealed class UploadFileResponse
{
    public Guid UploadId { get; init; }
    public string BlobId { get; init; }
    public string FileName { get; init; }
    public string ObjectKey { get; init; }
    public long Size { get; init; }

    private UploadFileResponse(
        Guid uploadId,
        string blobId,
        string fileName,
        string objectKey,
        long size)
    {
        UploadId = uploadId;
        BlobId = blobId;
        FileName = fileName;
        ObjectKey = objectKey;
        Size = size;
    }

    public static UploadFileResponse Create(
        Guid uploadId,
        string blobId,
        string fileName,
        string objectKey,
        long size)
    {
        return new UploadFileResponse(
            uploadId,
            blobId,
            fileName,
            objectKey,
            size);
    }
}
