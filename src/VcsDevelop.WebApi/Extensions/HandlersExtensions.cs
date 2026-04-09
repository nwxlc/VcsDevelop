using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.CommandHandlers;

namespace VcsDevelop.WebApi.Extensions;

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
