using Scalar.AspNetCore;
using Serilog;

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

        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();

        app.MapControllers();

        return app;
    }
}
