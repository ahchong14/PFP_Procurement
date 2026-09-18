using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.PurchaseRequests;
using PFP.Domain.Enums;

namespace PFP.Infrastructure.Persistence.Configurations.PurchaseRequests
{
    public sealed class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
    {
        public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
        {
            // Table
            builder.ToTable("purchaserequests");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Auto Increment
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            // Document Number
            builder.Property(x => x.DocNo)
                .HasMaxLength(50)
                .IsRequired();

            // Department
            builder.Property(x => x.Department)
                .HasMaxLength(100)
                .IsRequired();

            // Status
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(PRStatus.Quoting)
                .IsRequired();

            // PM Remarks
            builder.Property(x => x.PmRemarks)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Creditor Code Snapshot
            builder.Property(x => x.CreditorCode)
                .HasMaxLength(50)
                .IsRequired(false);

            // Creditor Name Snapshot
            builder.Property(x => x.CreditorName)
                .HasMaxLength(150)
                .IsRequired(false);

            // Row Version
            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Unique Document Number
            builder.HasIndex(x => x.DocNo)
                .IsUnique();

            // Requester
            builder.HasOne(x => x.Requester)
                .WithMany(x => x.PurchaseRequests)
                .HasForeignKey(x => x.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decided By User
            builder.HasOne(x => x.DecidedByUser)
                .WithMany()
                .HasForeignKey(x => x.DecidedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Selected Supplier Quote Copy
            builder.HasOne(x => x.SelectedSupplierCopy)
                .WithMany()
                .HasForeignKey(x => x.SelectedSupplierCopyId)
                .OnDelete(DeleteBehavior.NoAction);

            // Purchase Request -> Details
            builder.HasMany(x => x.Items)
                .WithOne(x => x.PurchaseRequest)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Purchase Request -> Supplier Quote Copies
            builder.HasMany(x => x.SupplierQuoteCopies)
                .WithOne(x => x.PurchaseRequest)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Purchase Request -> Request Quotations
            builder.HasMany(x => x.RequestQuotations)
                .WithOne(x => x.PurchaseRequest)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
