namespace VcsDevelop.Infrastructure.Options.Minio;

public interface IMinioSettings
{
    string Endpoint { get; }
    string AccessKey { get; }
    string SecretKey { get; }
    string BucketName { get; }
    bool Secure { get; }
}
