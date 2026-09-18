using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFP.Domain.Entities.Items;

namespace PFP.Infrastructure.Persistence.Configurations.Items
{
    public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            //Table 
            builder.ToTable("itmes");

            //Primary Key
            builder.HasKey(X => X.Id);

            // Auto Increment 
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            // Item Code
            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            // Item Name
            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            // Unit of Measurement
            builder.Property(x => x.Uom)
                .HasMaxLength(20)
                .IsRequired();

            // Reference Price
            builder.Property(x => x.RefPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // Unique Item Codw
            builder.HasIndex(x => x.Code)
                .IsUnique();

        }
    }
}
