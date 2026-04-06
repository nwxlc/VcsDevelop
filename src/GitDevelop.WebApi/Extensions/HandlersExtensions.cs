using GitDevelop.Application.Accounts.Abstractions;
using GitDevelop.Application.Accounts.CommandHandlers;

namespace GitDevelop.WebApi.Extensions;

public static class HandlersExtensions
{
    public static IServiceCollection AddHandlers(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IRegistrationCommandHandler, RegistrationCommandHandler>();

        return services;
    }
}
