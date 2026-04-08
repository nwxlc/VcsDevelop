using GitDevelop.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GitDevelop.WebApi.Extensions;

public static class MigrationExtensions
{
    public static async Task<WebApplication> ApplyMigrationsAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GitDevelopDbContext>();

        await dbContext.Database.MigrateAsync();

        return app;
    }
}
