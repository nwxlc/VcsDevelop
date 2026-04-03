using Scalar.AspNetCore;

namespace GitDevelop.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseHttpsRedirection();

        app.MapOpenApi();
        app.MapScalarApiReference();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();

        app.MapControllers();

        return app;
    }
}
