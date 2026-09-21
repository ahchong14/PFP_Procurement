using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.PurchaseRequests;

namespace PFP.Infrastructure.Persistence.Configurations.PurchaseRequests
{
    public sealed class PurchaseRequestDetailConfiguration : IEntityTypeConfiguration<PurchaseRequestDetail>
    {
        public void Configure(EntityTypeBuilder<PurchaseRequestDetail> builder)
        {
            // Table
            builder.ToTable("purchaserequestdetails");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Item Code
            builder.Property(x => x.ItemCode)
                .HasMaxLength(50)
                .IsRequired();

            // Description
            builder.Property(x => x.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            // Location
            builder.Property(x => x.Location)
                .HasMaxLength(200)
                .IsRequired(false);

            // Unit of Measurement
            builder.Property(x => x.Uom)
                .HasMaxLength(20)
                .IsRequired();

            // Quantity
            builder.Property(x => x.Qty)
                .HasPrecision(18, 3)
                .IsRequired();

            // Purchase Request
            builder.HasOne(x => x.PurchaseRequest)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
