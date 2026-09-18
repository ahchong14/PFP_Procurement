using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.Commons.Users;

namespace PFP.Infrastructure.Persistence.Configurations.Commons.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .HasMaxLength(255);

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Department)
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasMany(x => x.PurchaseRequests)
            .WithOne(x => x.Requester)
            .HasForeignKey(x => x.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.RQApprovals)
            .WithOne(x => x.Approver)
            .HasForeignKey(x => x.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
