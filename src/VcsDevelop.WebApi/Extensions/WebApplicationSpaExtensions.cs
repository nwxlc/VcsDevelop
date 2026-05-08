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
                   && !ctx.Request.Path.StartsWithSegments("/openapi")
                   && !ctx.Request.Path.StartsWithSegments("/api"),
            builder => 
            {
                builder.UseSpa(spa =>
                {
                    if (app.Environment.IsDevelopment())
                    {
                        spa.UseProxyToSpaDevelopmentServer("http://frontend:5173");
                    }
                });
            }
        );
        
        return app;
    }
}