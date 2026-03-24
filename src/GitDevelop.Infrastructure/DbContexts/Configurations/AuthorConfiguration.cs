using GitDevelop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitDevelop.Infrastructure.DbContexts.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("authors");

        builder
            .HasKey(author => author.Id)
            .HasName("pk_authors");

        // properties
        builder
            .Property(author => author.Id)
            .HasColumnName("id");

        builder
            .Property(author => author.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(author => author.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        // indexes
        builder
            .HasIndex(author => author.Email)
            .IsUnique()
            .HasDatabaseName("ux_authors_email");
    }
}
