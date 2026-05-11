using Minio;
using Minio.DataModel.Args;
using VcsDevelop.Infrastructure.Options.Minio;

namespace VcsDevelop.Infrastructure.Services;

public sealed class MinioBucketInitializer
{
    private readonly IMinioClient _minioClient;
    private readonly IMinioSettings _settings;

    public MinioBucketInitializer(
        IMinioClient minioClient,
        IMinioSettings settings)
    {
        ArgumentNullException.ThrowIfNull(minioClient);
        ArgumentNullException.ThrowIfNull(settings);

        _minioClient = minioClient;
        _settings = settings;
    }

    public async Task EnsureBucketExistsAsync(CancellationToken cancellationToken = default)
    {
        var bucketName = _settings.BucketName;
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new InvalidOperationException("MinIO bucket name is not configured.");
        }

        var bucketExists = await _minioClient
            .BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), cancellationToken)
            .ConfigureAwait(false);

        if (bucketExists)
        {
            return;
        }

        try
        {
            await _minioClient
                .MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), cancellationToken)
                .ConfigureAwait(false);
        }
        catch
        {
            var existsAfterFailure = await _minioClient
                .BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), cancellationToken)
                .ConfigureAwait(false);

            if (!existsAfterFailure)
            {
                throw;
            }
        }
    }
}
