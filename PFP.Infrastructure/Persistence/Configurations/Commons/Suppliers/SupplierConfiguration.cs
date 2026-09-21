using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.Commons.Suppliers;


namespace PFP.Infrastructure.Persistence.Configurations.Commons.Suppliers;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        // Table
        builder.ToTable("suppliers");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Name
        builder.Property(x => x.Name)
            .HasMaxLength(150);

        // Email
        builder.Property(x => x.Email)
            .HasMaxLength(255);

        // Contact
        builder.Property(x => x.Contact)
            .HasMaxLength(100);

        // Creditor Code
        builder.Property(x => x.CreditorCode)
            .HasMaxLength(50);

        // Account Status
        builder.Property(x => x.AccountStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Registration Token
        builder.Property(x => x.RegistrationToken)
            .HasMaxLength(100);

        // Password Hash
        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);

        // Unique Email
        builder.HasIndex(x => x.Email)
            .IsUnique();

        // Unique Creditor Code
        builder.HasIndex(x => x.CreditorCode)
            .IsUnique()
            .HasFilter("[CreditorCode] IS NOT NULL");

        // Unique Registration Token
        builder.HasIndex(x => x.RegistrationToken)
            .IsUnique()
            .HasFilter("[RegistrationToken] IS NOT NULL");

        // Supplier -> Request Quotations
        builder.HasMany(x => x.RequestQuotations)
            .WithOne(x => x.Supplier)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Supplier -> Purchase Orders
        builder.HasMany(x => x.PurchaseOrders)
            .WithOne(x => x.Supplier)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
