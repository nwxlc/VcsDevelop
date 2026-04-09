using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VcsDevelop.Domain.VcsObjects;

namespace VcsDevelop.Infrastructure.DbContexts.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("documents");

        builder
            .HasKey(document => document.Id)
            .HasName("pk_documents");

        // properties
        builder
            .Property(document => document.Id)
            .HasColumnName("id");

        builder
            .Property(document => document.Name)
            .HasMaxLength(200)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(document => document.DefaultBranchName)
            .HasMaxLength(120)
            .HasColumnName("default_branch_name")
            .IsRequired();

        builder
            .Property(document => document.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // ownerships
        builder
            .OwnsOne(document => document.Metadata, metadata =>
            {
                metadata
                    .Property(m => m.Title)
                    .HasColumnName("metadata_title")
                    .HasMaxLength(200)
                    .IsRequired();

                metadata
                    .Property(m => m.Description)
                    .HasColumnName("metadata_description")
                    .HasMaxLength(1000);

                metadata
                    .OwnsMany(m => m.Tags, tags =>
                    {
                        tags
                            .ToTable("document_tags");

                        tags
                            .WithOwner()
                            .HasForeignKey("document_id");

                        tags
                            .Property(tag => tag.Value)
                            .HasColumnName("tag")
                            .HasMaxLength(100)
                            .IsRequired();

                        tags
                            .HasKey("document_id", nameof(DocumentTag.Value));
                    });
            });
    }
}
