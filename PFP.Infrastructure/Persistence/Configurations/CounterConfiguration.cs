using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities;

namespace PFP.Infrastructure.Persistence.Configurations;

public sealed class CounterConfiguration
    : IEntityTypeConfiguration<Counter>
{
    public void Configure(EntityTypeBuilder<Counter> builder)
    {
        // Table
        builder.ToTable("counters");

        // Primary Key
        //
        // Counter uses Name as its primary key instead of Id.
        builder.HasKey(x => x.Name);

        // Counter Name
        //
        // Example:
        // "PR", "RQ", "PQ"
        builder.Property(x => x.Name)
            .HasMaxLength(10)
            .IsRequired();

        // Sequence Number
        //
        // New counters start from 0 by default.
        builder.Property(x => x.Seq)
            .HasDefaultValue(0)
            .IsRequired();
    }
}