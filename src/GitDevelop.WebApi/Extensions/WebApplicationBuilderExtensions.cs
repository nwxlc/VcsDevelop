using GitDevelop.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GitDevelop.WebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection service, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(configuration);

        service.AddOpenApi();

        service.AddRepository();
        service.AddDbContext<GitDevelopDbContext>(ConfigureNpgsql);

        return service;
    }

    private static void ConfigureNpgsql(
        IServiceProvider sp,
        DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("VCS-X");

        optionsBuilder.UseNpgsql(connectionString);
    }
}
