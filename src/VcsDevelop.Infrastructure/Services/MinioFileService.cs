using Minio;
using Minio.DataModel.Args;
using VcsDevelop.Application.VcsObjects.Files.Models;
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
        string? contentType,
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

        var args = new PutObjectArgs()
            .WithBucket(targetBucketName)
            .WithObject(key)
            .WithStreamData(stream)
            .WithObjectSize(length);

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            args.WithContentType(contentType);
        }

        await _minioClient
            .PutObjectAsync(args, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<DownloadFileResult> DownloadFileAsync(
        string key,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var targetBucketName = _options.BucketName;
        if (string.IsNullOrWhiteSpace(targetBucketName))
        {
            throw new InvalidOperationException("MinIO bucket name is not configured.");
        }

        var stat = await _minioClient.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(targetBucketName)
                    .WithObject(key),
                cancellationToken)
            .ConfigureAwait(false);

        var output = new MemoryStream();

        await _minioClient
            .GetObjectAsync(new GetObjectArgs()
                .WithBucket(targetBucketName)
                .WithObject(key)
                .WithCallbackStream(stream => stream.CopyTo(output)), cancellationToken)
            .ConfigureAwait(false);

        output.Position = 0;

        return new DownloadFileResult
        (
            output,
            string.IsNullOrWhiteSpace(stat.ContentType)
                ? "application/octet-stream"
                : stat.ContentType,
            stat.Size
        );
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
