using GitDevelop.Application.Accounts.Repositories;
using GitDevelop.Infrastructure.Repositories.Accounts;

namespace GitDevelop.WebApi.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection AddRepository(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}

