using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.PurchaseOrders;

namespace PFP.Infrastructure.Persistence.Configurations.PurchaseOrders;

public sealed class PurchaseOrderDetailConfiguration
    : IEntityTypeConfiguration<PurchaseOrderDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
    {
        // Table
        builder.ToTable("purchaseorderdetails");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Auto Increment
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Item Code
        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        // Description
        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        // Unit of Measurement
        builder.Property(x => x.Uom)
            .HasMaxLength(20)
            .IsRequired();

        // Quantity
        builder.Property(x => x.Qty)
            .HasPrecision(18, 3)
            .IsRequired();

        // Unit Price
        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        // Purchase Order
        builder.HasOne(x => x.PurchaseOrder)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}