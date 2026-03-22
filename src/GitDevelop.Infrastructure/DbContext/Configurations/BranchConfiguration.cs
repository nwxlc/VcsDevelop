using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitDevelop.Infrastructure.DbContext.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("branches");

        builder
            .HasKey(branch => branch.Id)
            .HasName("pk_branches");

        builder
            .Property(branch => branch.Id)
            .HasColumnName("id");

        builder
            .Property(branch => branch.RepositoryId)
            .HasColumnName("repository_id")
            .IsRequired();

        builder
            .Property(branch => branch.Name)
            .HasMaxLength(120)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(branch => branch.HeadCommitId)
            .HasColumnName("head_commit_id")
            .IsRequired();

        builder
            .Property(branch => branch.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder
            .HasIndex(b => new { b.RepositoryId, b.Name })
            .IsUnique()
            .HasDatabaseName("ux_branches_repository_name");

        builder
            .HasOne<Repository>()
            .WithMany()
            .HasForeignKey(b => b.RepositoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_branches_repository");

        builder
            .HasOne<Commit>()
            .WithMany()
            .HasForeignKey(b => b.HeadCommitId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_branches_head_commit");
    }
}
