using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities;

namespace PFP.Infrastructure.Persistence.Configurations;

public sealed class EmailSettingsConfiguration : IEntityTypeConfiguration<EmailSettings>
{
    public void Configure(EntityTypeBuilder<EmailSettings> builder)
    {
        // Table
        builder.ToTable("emailsettings");

        // Primary Key
        //
        // Single-row configuration - Id is always 1, set explicitly by the seed, not
        // database-generated (ValueGeneratedNever, rather than EF's default identity
        // convention, which would otherwise silently discard the seed's explicit value).
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasKey(x => x.Id);

        // Host
        builder.Property(x => x.Host)
            .HasMaxLength(255)
            .IsRequired();

        // Username
        builder.Property(x => x.Username)
            .HasMaxLength(255)
            .IsRequired();

        // Encrypted Password
        //
        // Protected via ISecretProtector before being stored; longer than a raw
        // password would need since Data Protection payloads are base64-encoded.
        builder.Property(x => x.EncryptedPassword)
            .HasMaxLength(1000)
            .IsRequired();

        // From Email
        builder.Property(x => x.FromEmail)
            .HasMaxLength(255)
            .IsRequired();

        // From Name
        builder.Property(x => x.FromName)
            .HasMaxLength(150)
            .IsRequired();
    }
}
