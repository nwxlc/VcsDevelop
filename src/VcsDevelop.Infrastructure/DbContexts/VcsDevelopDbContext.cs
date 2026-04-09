using Microsoft.EntityFrameworkCore;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Domain.VcsObjects;

namespace VcsDevelop.Infrastructure.DbContexts;

public sealed class VcsDevelopDbContext : DbContext
{
    public VcsDevelopDbContext(DbContextOptions<VcsDevelopDbContext> options)
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VcsDevelopDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
