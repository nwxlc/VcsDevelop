using Microsoft.Extensions.Options;

namespace VcsDevelop.Infrastructure.Options.Minio;

public sealed class MinioOptions : IMinioSettings
{
    public const string Name = "Minio";

    public string Endpoint { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public bool Secure { get; init; }
    public static IValidateOptions<MinioOptions> CreateValidator() => new MinioSettingsValidator();

    private sealed class MinioSettingsValidator : IValidateOptions<MinioOptions>
    {
        public ValidateOptionsResult Validate(string? name, MinioOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                throw new InvalidOperationException($"{Name}:{nameof(Endpoint)} is not configured.");
            }

            if (string.IsNullOrWhiteSpace(options.AccessKey))
            {
                throw new InvalidOperationException($"{Name}:{nameof(AccessKey)} is not configured.");
            }

            if (string.IsNullOrWhiteSpace(options.SecretKey))
            {
                throw new InvalidOperationException($"{Name}:{nameof(SecretKey)} is not configured.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
