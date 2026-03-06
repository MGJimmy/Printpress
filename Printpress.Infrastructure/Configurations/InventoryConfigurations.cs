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
            modelBuilder.Entity<PurchaseInvoice>().Configure();
            modelBuilder.Entity<PurchaseInvoiceLine>().Configure();
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

        private static void Configure(this EntityTypeBuilder<PurchaseInvoice> entity)
        {
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.SupplierName)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(x => x.AttachmentFilePath)
                .IsRequired()
                .HasMaxLength(500);
        }

        private static void Configure(this EntityTypeBuilder<PurchaseInvoiceLine> entity)
        {
            entity.SetSchemaTable(Schema);

            entity.HasOne(x => x.PurchaseInvoice)
                .WithMany(x => x.PurchaseInvoiceLines)
                .HasForeignKey(x => x.PurchaseInvoiceId);

            entity.HasOne(x => x.InventoryItem)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemId);
        }
    }
}
