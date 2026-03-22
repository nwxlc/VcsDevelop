using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitDevelop.Infrastructure.DbContext.Configurations;

public sealed class RepositoryConfiguration : IEntityTypeConfiguration<Repository>
{
    public void Configure(EntityTypeBuilder<Repository> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("repositories");

        builder
            .HasKey(repository => repository.Id)
            .HasName("pk_repositories");

        builder
            .Property(repository => repository.Id)
            .HasColumnName("id");

        builder
            .Property(repository => repository.Name)
            .HasMaxLength(200)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(repository => repository.DefaultBranchName)
            .HasMaxLength(120)
            .HasColumnName("default_branch_name")
            .IsRequired();

        builder
            .Property(repository => repository.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder
            .OwnsOne(repository => repository.Metadata, metadata =>
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
                            .ToTable("repository_tags");

                        tags
                            .WithOwner()
                            .HasForeignKey("repository_id");

                        tags
                            .Property(tag => tag.Value)
                            .HasColumnName("tag")
                            .HasMaxLength(100)
                            .IsRequired();

                        tags
                            .HasKey("repository_id", nameof(RepositoryTag.Value));
                    });
            });
    }
}
