using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitDevelop.Infrastructure.DbContext.Configurations;

public sealed class CommitConfiguration : IEntityTypeConfiguration<Commit>
{
    public void Configure(EntityTypeBuilder<Commit> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("commits");

        builder
            .HasKey(commit => commit.Id)
            .HasName("pk_commits");

        builder
            .Property(commit => commit.Id)
            .HasColumnName("id");

        builder
            .Property(commit => commit.RepositoryId)
            .HasColumnName("repository_id")
            .IsRequired();

        builder
            .Property(commit => commit.RootTreeId)
            .HasColumnName("root_tree_id")
            .IsRequired();

        builder
            .Property(c => c.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder
            .Property(branch => branch.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder
            .ComplexProperty(commit => commit.Hash, hash =>
            {
                hash
                    .Property(h => h.Value)
                    .HasColumnName("hash")
                    .HasColumnType("varbinary(32)")
                    .IsRequired();
            });

        builder
            .HasIndex(commit => commit.Hash.Value)
            .IsUnique()
            .HasDatabaseName("ux_commits_hash");

        builder
            .ComplexProperty(commit => commit.Message, propertyBuilder =>
            {
                propertyBuilder
                    .Property(commitMessage => commitMessage.Value)
                    .HasColumnType("text")
                    .HasColumnName("message")
                    .IsRequired();
            });

        //todo сделоать конфигурацию для ParentId

        builder
            .HasOne<Repository>()
            .WithMany()
            .HasForeignKey(commit => commit.RepositoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_commits_repository");

        builder
            .HasOne<Tree>()
            .WithMany()
            .HasForeignKey(commit => commit.RootTreeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_commits_root_tree");

        builder
            .HasOne<Author>()
            .WithMany()
            .HasForeignKey(commit => commit.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_commits_author");

        builder
            .HasIndex(c => c.RepositoryId)
            .HasDatabaseName("ix_commits_repository_id");

        builder
            .HasIndex(c => c.AuthorId)
            .HasDatabaseName("ix_commits_author_id");

        builder
            .HasIndex(c => c.CreatedAt)
            .HasDatabaseName("ix_commits_created_at");
    }
}
