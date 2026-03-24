using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitDevelop.Infrastructure.DbContexts;

public sealed class GitDevelopDbContext : DbContext
{
    public GitDevelopDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Commit> Commits => Set<Commit>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<Blob> Blobs => Set<Blob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GitDevelopDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
