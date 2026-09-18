using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities;

namespace PFP.Infrastructure.Persistence.Configurations;

public sealed class ApprovalSettingConfiguration
    : IEntityTypeConfiguration<ApprovalSetting>
{
    public void Configure(EntityTypeBuilder<ApprovalSetting> builder)
    {
        // Table
        builder.ToTable("approvalsettings");

        // Primary Key
        //
        // ApprovalSetting uses Level as its primary key
        // instead of an auto-increment Id.
        builder.HasKey(x => x.Level);

        // Approval Level
        builder.Property(x => x.Level)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        // Approver Role
        builder.Property(x => x.ApproverRole)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // Minimum Approval Amount
        builder.Property(x => x.MinAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        // Maximum Approval Amount
        //
        // NULL means there is no maximum amount limit.
        builder.Property(x => x.MaxAmount)
            .HasPrecision(18, 2)
            .IsRequired(false);
    }
}