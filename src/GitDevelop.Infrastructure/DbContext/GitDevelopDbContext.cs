using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitDevelop.Infrastructure.DbContext;

public sealed class GitDevelopDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Commit> Commits => Set<Commit>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<Blob> Blobs => Set<Blob>();

    public GitDevelopDbContext(DbContextOptions<GitDevelopDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GitDevelopDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
