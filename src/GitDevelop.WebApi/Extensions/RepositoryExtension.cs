using GitDevelop.Application.Accounts.Repositories;
using GitDevelop.Infrastructure.Auth;
using GitDevelop.Infrastructure.Repositories.Accounts;
using StackExchange.Redis;

namespace GitDevelop.WebApi.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection AddRepository(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IAccountRepository, AccountRepository>();

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
