using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.RequestQuotations;

namespace PFP.Infrastructure.Persistence.Configurations.RequestQuotations;

public sealed class RequestQuotationConfiguration
    : IEntityTypeConfiguration<RequestQuotation>
{
    public void Configure(EntityTypeBuilder<RequestQuotation> builder)
    {
        // Table
        builder.ToTable("requestquotations");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Auto Increment
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

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

        // Requires Level 2 Approval
        builder.Property(x => x.RequiresL2)
            .IsRequired();

        // Created At
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Row Version
        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        // Unique Document Number
        builder.HasIndex(x => x.DocNo)
            .IsUnique();

        // Purchase Request
        builder.HasOne(x => x.PurchaseRequest)
            .WithMany(x => x.RequestQuotations)
            .HasForeignKey(x => x.PurchaseRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Supplier
        builder.HasOne(x => x.Supplier)
            .WithMany()
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Request Quotation -> Details
        builder.HasMany(x => x.Items)
            .WithOne(x => x.RequestQuotation)
            .HasForeignKey(x => x.RequestQuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Request Quotation -> Approvals
        builder.HasMany(x => x.Approvals)
            .WithOne(x => x.RequestQuotation)
            .HasForeignKey(x => x.RequestQuotationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}