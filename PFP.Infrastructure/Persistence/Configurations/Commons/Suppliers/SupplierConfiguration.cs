using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.Commons.Suppliers;


namespace PFP.Infrastructure.Persistence.Configurations.Commons.Suppliers;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .HasMaxLength(255);

        builder.Property(x => x.Contact)
            .HasMaxLength(100);

        builder.Property(x => x.CreditorCode)
            .HasMaxLength(50);

        builder.Property(x => x.AccountStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.RegistrationToken)
            .HasMaxLength(100);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.CreditorCode)
            .IsUnique()
            .HasFilter("[CreditorCode] IS NOT NULL");

        builder.HasIndex(x => x.RegistrationToken)
            .IsUnique()
            .HasFilter("[RegistrationToken] IS NOT NULL");

        builder.HasMany(x => x.RequestQuotations)
            .WithOne(x => x.Supplier)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PurchaseOrders)
            .WithOne(x => x.Supplier)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
