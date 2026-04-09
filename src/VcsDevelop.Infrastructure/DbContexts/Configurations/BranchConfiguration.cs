using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VcsDevelop.Domain.VcsObjects;

namespace VcsDevelop.Infrastructure.DbContexts.Configurations;

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

        // properties
        builder
            .Property(branch => branch.DocumentId)
            .HasColumnName("document_id")
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
            .HasIndex(b => new { b.DocumentId, b.Name })
            .IsUnique()
            .HasDatabaseName("ux_branches_document_name");

        // ownerships
        builder
            .HasOne<Document>()
            .WithMany()
            .HasForeignKey(b => b.DocumentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_branches_document");

        builder
            .HasOne<Commit>()
            .WithMany()
            .HasForeignKey(b => b.HeadCommitId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_branches_head_commit");
    }
}
