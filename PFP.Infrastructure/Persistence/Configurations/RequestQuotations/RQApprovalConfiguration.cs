using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.RequestQuotations;

namespace PFP.Infrastructure.Persistence.Configurations.RequestQuotations;

public sealed class RQApprovalConfiguration
    : IEntityTypeConfiguration<RQApproval>
{
    public void Configure(EntityTypeBuilder<RQApproval> builder)
    {
        // Table
        builder.ToTable("rqapprovals");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Approval Level
        builder.Property(x => x.Level)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        // Approval Action
        builder.Property(x => x.Action)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Remark
        builder.Property(x => x.Remark)
            .HasMaxLength(1000)
            .IsRequired(false);

        // Timestamp
        builder.Property(x => x.Timestamp)
            .IsRequired();

        // Request Quotation
        builder.HasOne(x => x.RequestQuotation)
            .WithMany(x => x.Approvals)
            .HasForeignKey(x => x.RequestQuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Approver
        builder.HasOne(x => x.Approver)
            .WithMany(x => x.RQApprovals)
            .HasForeignKey(x => x.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}