using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.CommandHandlers;
using VcsDevelop.Application.VcsObjects.Abstractions;
using VcsDevelop.Application.VcsObjects.CommandHandlers;

namespace VcsDevelop.WebApi.Extensions;

public static class HandlersExtensions
{
    public static IServiceCollection AddHandlers(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IRegistrationCommandHandler, RegistrationCommandHandler>();
        services.AddScoped<ILoginCommandHandler, LoginCommandHandler>();
        services.AddScoped<ILogoutCommandHandler, LogoutCommandHandler>();

        services.AddScoped<ICreateDocumentHandler, CreateDocumentHandler>();

        return services;
    }
}
