using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitDevelop.Infrastructure.DbContext.Configurations;

public class TreeConfiguration : IEntityTypeConfiguration<Tree>
{
    public void Configure(EntityTypeBuilder<Tree> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("trees");

        builder
            .HasKey(tree => tree.Id)
            .HasName("pk_trees");

        builder
            .Property(tree => tree.Id)
            .HasColumnName("tree_id")
            .IsRequired();

        builder
            .ComplexProperty(tree => tree.Hash, hash =>
            {
                hash
                    .Property(contentHash => contentHash.Value)
                    .HasColumnName("hash")
                    .HasColumnType("varbinary(32)")
                    .IsRequired();
            });

        builder
            .HasIndex(tree => tree.Hash.Value)
            .IsUnique()
            .HasDatabaseName("ux_trees_hash");

        builder
            .OwnsMany(tree => tree.Entries, entries =>
            {
                entries
                    .ToTable("tree_entries");

                entries
                    .WithOwner()
                    .HasForeignKey("tree_id");

                entries
                    .Property<int>("id")
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entries
                    .HasKey("tree_id", "id")
                    .HasName("pk_tree_entries");

                entries
                    .Property(entry => entry.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();

                entries
                    .Property(entry => entry.ObjectId)
                    .HasColumnName("object_id")
                    .HasMaxLength(64)
                    .IsRequired();

                entries
                    .HasIndex("tree_id", "name")
                    .IsUnique()
                    .HasDatabaseName("ix_tree_entries_tree_name");
            });
    }
}
