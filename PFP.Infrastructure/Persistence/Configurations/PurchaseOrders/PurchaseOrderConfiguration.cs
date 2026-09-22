using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.PurchaseOrders;

namespace PFP.Infrastructure.Persistence.Configurations.PurchaseOrders;

public sealed class PurchaseOrderConfiguration
    : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        // Table
        builder.ToTable("purchaseorders");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Document Number
        builder.Property(x => x.DocNo)
            .HasMaxLength(50)
            .IsRequired();

        // Total Amount
        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        // Status
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // New Supplier Flag
        builder.Property(x => x.IsNewSupplier)
            .IsRequired();

        // Created At
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Synced At
        builder.Property(x => x.SyncedAt)
            .IsRequired(false);

        // AutoCount PO Reference
        builder.Property(x => x.AutoCountPORef)
            .HasMaxLength(100)
            .IsRequired(false);

        // AutoCount Creditor Reference
        builder.Property(x => x.AutoCountCreditorRef)
            .HasMaxLength(100)
            .IsRequired(false);

        // Synchronization Error
        builder.Property(x => x.SyncError)
            .HasMaxLength(2000)
            .IsRequired(false);

        // Synchronization Attempts
        builder.Property(x => x.SyncAttempts)
            .HasDefaultValue(0)
            .IsRequired();

        // Row Version
        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        // Unique Document Number
        builder.HasIndex(x => x.DocNo)
            .IsUnique();

        // Unique Request Quotation
        //
        // Ensures one RequestQuotation can only be linked
        // to one PurchaseOrder.
        builder.HasIndex(x => x.RequestQuotationId)
            .IsUnique();

        // Request Quotation
        builder.HasOne(x => x.RequestQuotation)
            .WithOne(x => x.PurchaseOrder)
            .HasForeignKey<PurchaseOrder>(x => x.RequestQuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Supplier
        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.PurchaseOrders)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Purchase Order -> Details
        builder.HasMany(x => x.Items)
            .WithOne(x => x.PurchaseOrder)
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}