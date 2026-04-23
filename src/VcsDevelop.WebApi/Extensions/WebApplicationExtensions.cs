using Hellang.Middleware.ProblemDetails;
using Scalar.AspNetCore;
using Serilog;

namespace VcsDevelop.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseSerilogRequestLogging();

        app.UseProblemDetails();

        var useHttpsRedirection = app.Configuration.GetValue("Http:UseHttpsRedirection", true);
        if (useHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapOpenApi();
        app.MapScalarApiReference();

        app.MapControllers();

        return app;
    }
}
