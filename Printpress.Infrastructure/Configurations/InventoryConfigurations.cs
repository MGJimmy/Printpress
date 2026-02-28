using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain;

namespace Printpress.Infrastructure
{
    public static class InventoryConfigurations
    {
        private const string Schema = "Inventory";

        public static void ConfigureInventory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>().Configure();
            modelBuilder.Entity<InventoryTransaction>().Configure();
            modelBuilder.Entity<InventoryItemCategory_LKP>().Configure();
        }

        private static void Configure(this EntityTypeBuilder<InventoryItem> entity)
        {
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(x => x.InventoryItemCategory_LKP)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemCategoryId);

            entity.Ignore(x => x.InventoryItemCategory);
        }

        private static void Configure(this EntityTypeBuilder<InventoryTransaction> entity)
        {
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.HasOne(x => x.InventoryItem)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemId);
        }

        private static void Configure(this EntityTypeBuilder<InventoryItemCategory_LKP> entity)
        {
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
