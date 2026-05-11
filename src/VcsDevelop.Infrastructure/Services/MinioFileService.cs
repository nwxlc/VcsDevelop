using Minio;
using Minio.DataModel.Args;
using VcsDevelop.Application.VcsObjects.Services;
using VcsDevelop.Infrastructure.Options.Minio;

namespace VcsDevelop.Infrastructure.Services;

public sealed class MinioFileService : IFileService
{
    private readonly IMinioClient _minioClient;
    private readonly IMinioSettings _options;

    public MinioFileService(
        IMinioClient minioClient,
        IMinioSettings options)
    {
        ArgumentNullException.ThrowIfNull(minioClient);
        ArgumentNullException.ThrowIfNull(options);

        _minioClient = minioClient;
        _options = options;
    }

    public async Task UploadFileAsync(
        Stream stream,
        string key,
        long length,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        var targetBucketName = _options.BucketName;

        if (string.IsNullOrWhiteSpace(targetBucketName))
        {
            throw new InvalidOperationException("MinIO bucket name is not configured.");
        }

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        await _minioClient
            .PutObjectAsync(new PutObjectArgs()
                .WithBucket(targetBucketName)
                .WithObject(key)
                .WithStreamData(stream)
                .WithObjectSize(length), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task DeleteFileAsync(
        string key,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var targetBucketName = _options.BucketName;
        if (string.IsNullOrWhiteSpace(targetBucketName))
        {
            return;
        }

        await _minioClient
            .RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(targetBucketName)
                .WithObject(key), cancellationToken)
            .ConfigureAwait(false);
    }
}
