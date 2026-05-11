using Microsoft.Extensions.Options;
using Minio;
using VcsDevelop.Infrastructure.Options.Minio;
using VcsDevelop.Infrastructure.Services;

namespace VcsDevelop.WebApi.Extensions;

public static class AddMinioExtensions
{
    public static IServiceCollection AddMinioServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<MinioOptions>().Bind(configuration.GetSection(MinioOptions.Name));

        services.AddSingleton(MinioOptions.CreateValidator());

        services.AddSingleton<IMinioSettings>(provider =>
            provider.GetRequiredService<IOptions<MinioOptions>>().Value);

        services.AddSingleton<IMinioClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MinioOptions>>().Value;

            return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.Secure)
                .Build();
        });

        services.AddSingleton<MinioBucketInitializer>();

        return services;
    }
}
