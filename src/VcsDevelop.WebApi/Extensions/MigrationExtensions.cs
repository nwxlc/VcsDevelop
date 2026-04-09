using Microsoft.EntityFrameworkCore;
using VcsDevelop.Infrastructure.DbContexts;

namespace VcsDevelop.WebApi.Extensions;

public static class MigrationExtensions
{
    public static async Task<WebApplication> ApplyMigrationsAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VcsDevelopDbContext>();

        await dbContext.Database.MigrateAsync();

        return app;
    }
}
