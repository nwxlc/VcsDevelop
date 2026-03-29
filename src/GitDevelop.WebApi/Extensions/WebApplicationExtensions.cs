using Scalar.AspNetCore;

namespace GitDevelop.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapOpenApi();
        app.MapScalarApiReference();
        
        return app;
    }
}
