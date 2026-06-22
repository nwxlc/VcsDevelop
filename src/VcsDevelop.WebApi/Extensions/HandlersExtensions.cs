using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.CommandHandlers;
using VcsDevelop.Application.Accounts.QueryHandlers;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.CommandHandlers;
using VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;
using VcsDevelop.Application.VcsObjects.Files.Abstractions;
using VcsDevelop.Application.VcsObjects.Files.CommandHandlers;
using VcsDevelop.Application.VcsObjects.Files.QueryHandlers;

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
        services.AddScoped<IRefreshAccessTokenCommandHandler, RefreshAccessTokenCommandHandler>();

        services.AddScoped<ICreateDocumentHandler, CreateDocumentHandler>();
        services.AddScoped<IGetDocumentByIdHandler, GetDocumentByIdHandler>();
        services.AddScoped<IGetDocumentsHandler, GetDocumentsHandler>();
        services.AddScoped<IGetRepositoryTreeHandler, GetRepositoryTreeHandler>();
        services.AddScoped<IGetRepositoryBlobHandler, GetRepositoryBlobHandler>();
        services.AddScoped<IGetRepositoryDiffHandler, GetRepositoryDiffHandler>();
        services.AddScoped<IUploadFileHandler, UploadFileHandler>();
        services.AddScoped<IDownloadFileHandler, DownloadFileHandler>();
        services.AddScoped<IStageDocumentFileHandler, StageDocumentFileHandler>();
        services.AddScoped<ICommitDocumentHandler, CommitDocumentHandler>();
        services.AddScoped<IGetDocumentLogHandler, GetDocumentLogHandler>();
        services.AddScoped<IRevertDocumentHandler, RevertDocumentHandler>();

        return services;
    }
}
