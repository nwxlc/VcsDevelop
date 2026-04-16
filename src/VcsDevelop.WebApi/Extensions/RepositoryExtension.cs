using StackExchange.Redis;
using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.Auth;
using VcsDevelop.Infrastructure.Repositories.Accounts;
using VcsDevelop.Infrastructure.Repositories.VcsObjects;

namespace VcsDevelop.WebApi.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection AddRepository(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IAccountRepository, AccountRepository>();

        services.AddScoped<IDocumentRepository, DocumentRepository>();

        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("Redis")
                                   ?? throw new InvalidOperationException("Redis string not found");

            return ConnectionMultiplexer.Connect(connectionString);
        });

        services.AddSingleton<IRefreshTokenRepository, RedisRefreshTokenRepository>();

        return services;
    }
}
