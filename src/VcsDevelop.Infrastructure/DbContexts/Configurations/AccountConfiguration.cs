using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VcsDevelop.Domain.Accounts;

namespace VcsDevelop.Infrastructure.DbContexts.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("accounts");

        builder
            .HasKey(account => account.Id)
            .HasName("pk_accounts");

        // properties
        builder
            .Property(account => account.Id)
            .HasColumnName("id");

        builder
            .Property(account => account.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(account => account.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        builder
            .Property(account => account.Bio)
            .HasColumnName("bio")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder
            .Property(account => account.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(500)
            .IsRequired(false);

        builder
            .Property(account => account.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(account => account.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.ComplexProperty(account => account.Password, password =>
        {
            password.Property(p => p.HashedValue)
                .HasColumnName("password_hash")
                .HasMaxLength(300)
                .IsRequired();
        });

        // indexes
        builder
            .HasIndex(account => account.Email)
            .IsUnique()
            .HasDatabaseName("ux_accounts_email");
    }
}
