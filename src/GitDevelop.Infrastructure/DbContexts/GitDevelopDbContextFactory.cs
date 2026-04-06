using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GitDevelop.Infrastructure.DbContexts;

public class GitDevelopDbContextFactory : IDesignTimeDbContextFactory<GitDevelopDbContext>
{
    public GitDevelopDbContext CreateDbContext(string[] args)
    {
        var configsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "settings");

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine(configsFolderPath, "appsettings.json"), optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        configurationBuilder = configurationBuilder
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        var configuration = configurationBuilder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<GitDevelopDbContext>();
        var connectionString = configuration.GetConnectionString("VCS-X");

        optionsBuilder.UseNpgsql(connectionString);

        return new GitDevelopDbContext(optionsBuilder.Options);
    }
}
