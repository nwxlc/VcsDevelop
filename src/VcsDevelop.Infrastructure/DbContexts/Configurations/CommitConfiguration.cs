using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Domain.VcsObjects;

namespace VcsDevelop.Infrastructure.DbContexts.Configurations;

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

        // properties
        builder
            .Property(commit => commit.Id)
            .HasColumnName("id")
            .HasColumnType("char(40)");

        builder
            .Property(commit => commit.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();

        builder
            .Property(commit => commit.RootTreeId)
            .HasColumnName("root_tree_id")
            .HasColumnType("char(40)")
            .IsRequired();

        builder
            .Property(commit => commit.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder
            .Property(commit => commit.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder
            .ComplexProperty(commit => commit.Message, propertyBuilder =>
            {
                propertyBuilder
                    .Property(commitMessage => commitMessage.Value)
                    .HasColumnType("text")
                    .HasColumnName("message")
                    .IsRequired();
            });

        // ownerships
        builder
            .HasOne<Document>()
            .WithMany()
            .HasForeignKey(commit => commit.DocumentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_commits_document");

        builder
            .HasOne<Tree>()
            .WithMany()
            .HasForeignKey(commit => commit.RootTreeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_commits_root_tree");

        builder
            .HasOne<Account>()
            .WithMany()
            .HasForeignKey(commit => commit.AccountId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_commits_account");

        builder
            .OwnsMany(commit => commit.ParentIds, parents =>
            {
                parents
                    .ToTable("commit_parents");

                parents
                    .WithOwner().HasForeignKey("commit_id");

                parents
                    .Property(parent => parent.ParentId)
                    .HasColumnName("parent_id")
                    .IsRequired();

                parents
                    .HasKey("commit_id", nameof(CommitParent.ParentId));

                parents
                    .HasIndex(parent => parent.ParentId)
                    .HasDatabaseName("ix_commit_parents_parent_id");
            });

        // indexes
        builder
            .HasIndex(c => c.DocumentId)
            .HasDatabaseName("ix_commits_document_id");

        builder
            .HasIndex(c => c.AccountId)
            .HasDatabaseName("ix_commits_account_id");

        builder
            .HasIndex(c => c.CreatedAt)
            .HasDatabaseName("ix_commits_created_at");
    }
}
