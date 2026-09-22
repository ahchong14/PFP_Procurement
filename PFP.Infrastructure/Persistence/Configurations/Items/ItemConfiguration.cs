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
            builder.ToTable("items");

            //Primary Key
            builder.HasKey(X => X.Id);

            // Item Code
            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            // Item Name
            builder.Property(x => x.Name)
                .HasMaxLength(150)
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
