using GitDevelop.Domain.Accounts;
using GitDevelop.Domain.GitObjects;
using Microsoft.EntityFrameworkCore;

namespace GitDevelop.Infrastructure.DbContexts;

public sealed class GitDevelopDbContext : DbContext
{
    public GitDevelopDbContext(DbContextOptions<GitDevelopDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Commit> Commits => Set<Commit>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<Blob> Blobs => Set<Blob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GitDevelopDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
