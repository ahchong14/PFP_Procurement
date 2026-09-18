using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.SupplierQuoteCopys;
using PFP.Domain.Enums;

namespace PFP.Infrastructure.Persistence.Configurations.SupplierQuotes
{
    public sealed class SupplierQuoteCopyConfiguration : IEntityTypeConfiguration<SupplierQuoteCopy>
    {
        public void Configure(EntityTypeBuilder<SupplierQuoteCopy> builder)
        {
            // Table 
            builder.ToTable("supplierquotecopies");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Auto Increment 
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            // Public Submission Token
            builder.Property(x => x.Token)
                .HasMaxLength(100)
                .IsRequired();

            // Status
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(Copystatus.Pending)
                .IsRequired();

            //Remarks
            builder.Property(x => x.Remarks)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Total Amount
            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            // Sent At
            builder.Property(x => x.SentAt)
                .IsRequired();

            // Submitted At
            builder.Property(x => x.SubmittedAt)
                .IsRequired(false);

            // Unique Submission Token
            builder.HasIndex(x => x.Token)
                .IsUnique();

            // Purchase Request
            builder.HasOne(x => x.PurchaseRequest)
                .WithMany(x => x.SupplierQuoteCopies)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Supplier
            builder.HasOne(x => x.Supplier)
                .WithMany()
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
