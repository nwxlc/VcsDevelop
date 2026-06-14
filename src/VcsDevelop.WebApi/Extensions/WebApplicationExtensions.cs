using Hellang.Middleware.ProblemDetails;
using Serilog;
using VcsDevelop.Core.Logging;
using VcsDevelop.Infrastructure.Services;

namespace VcsDevelop.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseSerilogRequestLogging();

        app.InitializeLogManager();

        var useHttpsRedirection = app.Configuration.GetValue("Http:UseHttpsRedirection", true);
        if (useHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();

        app.UseCors("Client");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseProblemDetails();

        app.MapControllers();

        app.ConfigureSpa();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapFallbackToFile("index.html");

        return app;
    }

    public static async Task<WebApplication> EnsureMinioBucketExistsAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<MinioBucketInitializer>();
        await initializer.EnsureBucketExistsAsync().ConfigureAwait(false);

        return app;
    }

    private static WebApplication InitializeLogManager(this WebApplication app)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        LogManager.Initialize(loggerFactory);
        return app;
    }
}
