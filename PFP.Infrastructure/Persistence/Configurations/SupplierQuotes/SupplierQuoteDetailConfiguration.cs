using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.SupplierQuoteCopys;

namespace PFP.Infrastructure.Persistence.Configurations.SupplierQuotes;

public sealed class SupplierQuoteDetailConfiguration
    : IEntityTypeConfiguration<SupplierQuoteDetail>
{
    public void Configure(EntityTypeBuilder<SupplierQuoteDetail> builder)
    {
        // Table
        builder.ToTable("supplierquotedetails");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Auto Increment
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Unit Price
        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        // Supplier Quote Copy
        builder.HasOne(x => x.SupplierQuoteCopy)
            .WithMany(x => x.QuotedDetail)
            .HasForeignKey(x => x.SupplierQuoteCopyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Purchase Request Detail
        builder.HasOne(x => x.PurchaseRequestDetail)
            .WithMany(x => x.QuotedBy)
            .HasForeignKey(x => x.PurchaseRequestItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}