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
