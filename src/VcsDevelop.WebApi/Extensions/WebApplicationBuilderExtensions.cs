using Microsoft.EntityFrameworkCore;
using VcsDevelop.Infrastructure.DbContexts;

namespace VcsDevelop.WebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection service, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(configuration);

        service.AddControllers();

        service.AddOpenApi();

        service.AddAuthentication(configuration);

        service.AddRepository();

        service.AddHandlers();

        service.AddHttpContextAccessor();
        service.AddTokenProvider(configuration);

        service.AddDbContext<VcsDevelopDbContext>(ConfigureNpgsql);

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
