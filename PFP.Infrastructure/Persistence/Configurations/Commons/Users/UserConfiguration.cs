using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Infrastructure.Persistence.Configurations.Commons.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table
        builder.ToTable("users");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Name
        builder.Property(x => x.Name)
            .HasMaxLength(150);

        // Email
        builder.Property(x => x.Email)
            .HasMaxLength(255);

        // Role
        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        // Department
        builder.Property(x => x.Department)
            .HasMaxLength(100);

        // Active Status
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // Password Hash
        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);

        // Unique Email
        builder.HasIndex(x => x.Email)
            .IsUnique();

        // User -> Purchase Requests
        builder.HasMany(x => x.PurchaseRequests)
            .WithOne(x => x.Requester)
            .HasForeignKey(x => x.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> RQ Approvals
        builder.HasMany(x => x.RQApprovals)
            .WithOne(x => x.Approver)
            .HasForeignKey(x => x.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
