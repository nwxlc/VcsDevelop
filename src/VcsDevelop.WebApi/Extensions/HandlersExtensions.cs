using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.CommandHandlers;
using VcsDevelop.Application.Accounts.QueryHandlers;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.CommandHandlers;
using VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;

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
        services.AddScoped<IGetAccountByIdHandler, GetAccountByIdHandler>();
        services.AddScoped<IUpdateAccountHandler, UpdateAccountHandler>();

        services.AddScoped<ICreateDocumentHandler, CreateDocumentHandler>();
        services.AddScoped<IGetDocumentByIdHandler, GetDocumentByIdHandler>();
        services.AddScoped<IUploadFileHandler, UploadFileHandler>();

        return services;
    }
}
