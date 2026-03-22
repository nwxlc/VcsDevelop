using GitDevelop.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GitDevelop.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection service, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(configuration);

        service.AddDbContext<GitDevelopDbContext>(ConfigureNpgsql);
        
        return service;
    }

    private static void ConfigureNpgsql(
        IServiceProvider sp,
        DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("GitDevelop");

        optionsBuilder.UseNpgsql(connectionString);
    }
}
