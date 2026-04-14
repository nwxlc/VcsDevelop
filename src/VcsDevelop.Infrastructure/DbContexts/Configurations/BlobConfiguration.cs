using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VcsDevelop.Domain.VcsObjects;

namespace VcsDevelop.Infrastructure.DbContexts.Configurations;

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
            .HasColumnName("id")
            .HasColumnType("char(40)");

        builder
            .Property(blob => blob.Size)
            .HasColumnName("size")
            .IsRequired();

        builder
            .Property(blob => blob.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
