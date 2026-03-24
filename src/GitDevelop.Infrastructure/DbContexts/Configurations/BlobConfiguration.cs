using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitDevelop.Infrastructure.DbContexts.Configurations;

public sealed class BlobConfiguration : IEntityTypeConfiguration<Blob>
{
    public void Configure(EntityTypeBuilder<Blob> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("blobs");

        builder
            .HasKey(blob => blob.Id)
            .HasName("pk_blobs");

        // properties
        builder
            .Property(blob => blob.Id)
            .HasColumnName("id");

        builder
            .Property(blob => blob.Size)
            .HasColumnName("size")
            .IsRequired();

        builder
            .Property(blob => blob.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder
            .ComplexProperty(blob => blob.Hash, hash =>
            {
                hash
                    .Property(contentHash => contentHash.Value)
                    .HasColumnName("hash")
                    .HasColumnType("varbinary(32)")
                    .HasMaxLength(32)
                    .IsRequired();
            });

        // indexes
        builder
            .HasIndex(blob => blob.Hash.Value)
            .IsUnique()
            .HasDatabaseName("ux_blobs_hash");
    }
}
