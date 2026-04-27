using VcsDevelop.Application.VcsObjects.Services;
using VcsDevelop.Infrastructure.Services;

namespace VcsDevelop.WebApi.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddFileServices(this IServiceCollection services)
    {
        services.AddScoped<ICompressionService, ZLibCompressionService>();
        services.AddScoped<IFileService, MinioFileService>();
        services.AddScoped<IHashService, HashService>();

        return services;
    }
}
