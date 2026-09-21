using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.RequestQuotations;

namespace PFP.Infrastructure.Persistence.Configurations.RequestQuotations;

public sealed class RequestQuotationDetailConfiguration
    : IEntityTypeConfiguration<RequestQuotationDetail>
{
    public void Configure(EntityTypeBuilder<RequestQuotationDetail> builder)
    {
        // Table
        builder.ToTable("requestquotationdetails");

        // Primary Key
        builder.HasKey(x => x.Id);

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

        // Request Quotation
        builder.HasOne(x => x.RequestQuotation)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.RequestQuotationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}