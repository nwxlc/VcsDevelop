using Hellang.Middleware.ProblemDetails;
using Scalar.AspNetCore;
using Serilog;
using VcsDevelop.Infrastructure.Services;

namespace VcsDevelop.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseSerilogRequestLogging();

        var useHttpsRedirection = app.Configuration.GetValue("Http:UseHttpsRedirection", true);
        if (useHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseProblemDetails();

        app.MapOpenApi();
        app.MapScalarApiReference();

        app.MapControllers();

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
}
