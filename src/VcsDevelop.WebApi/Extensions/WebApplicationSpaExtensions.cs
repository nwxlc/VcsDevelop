using Scalar.AspNetCore;

namespace VcsDevelop.WebApi.Extensions;

public static class WebApplicationSpaExtensions
{
    public static WebApplication ConfigureSpa(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseWhen(
            ctx => !ctx.Request.Path.StartsWithSegments("/scalar")
                   && !ctx.Request.Path.StartsWithSegments("/openapi"),
            builder => builder.UseSpa(spa =>
            {
                spa.UseProxyToSpaDevelopmentServer("http://localhost:5237");
            })
        );
        
        return app;
    }
}