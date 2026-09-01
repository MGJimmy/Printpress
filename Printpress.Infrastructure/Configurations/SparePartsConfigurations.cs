using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain;

namespace Printpress.Infrastructure
{
    public static class SparePartsConfigurations
    {
        private const string Schema = "SpareParts";
        private const string SellingInvoiceNumberSequence = "SparePartSellingInvoiceNumber";

        public static void ConfigureSpareParts(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>(SellingInvoiceNumberSequence, Schema).StartsAt(1).IncrementsBy(1);

            modelBuilder.Entity<SparePartInventoryItem>().Configure();
            modelBuilder.Entity<SparePartInventoryTransaction>().Configure();
            modelBuilder.Entity<SparePartPurchaseInvoice>().Configure();
            modelBuilder.Entity<SparePartPurchaseInvoiceLine>().Configure();
            modelBuilder.Entity<SparePartSellingInvoice>().Configure(Schema, SellingInvoiceNumberSequence);
            modelBuilder.Entity<SparePartSellingInvoiceLine>().Configure();
        }

        private static void Configure(this EntityTypeBuilder<SparePartInventoryItem> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasMany(x => x.InventoryTransactions)
                .WithOne(x => x.InventoryItem)
                .HasForeignKey(x => x.InventoryItemId);
        }

        private static void Configure(this EntityTypeBuilder<SparePartInventoryTransaction> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);
        }

        private static void Configure(this EntityTypeBuilder<SparePartPurchaseInvoice> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.SupplierName)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(x => x.AttachmentFilePath)
                .HasMaxLength(500);

            entity.HasMany(x => x.PurchaseInvoiceLines)
                .WithOne(x => x.PurchaseInvoice)
                .HasForeignKey(x => x.PurchaseInvoiceId);

            entity.Property(x => x.PaidAmount)
                .IsRequired()
                .HasDefaultValue(0m);

            entity.Property(x => x.IsGoodsReceived)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(x => x.IsVoided)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(x => x.VoidReason)
                .HasMaxLength(500);

            entity.Property(x => x.VoidedBy)
                .HasMaxLength(100);
        }

        private static void Configure(this EntityTypeBuilder<SparePartPurchaseInvoiceLine> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.HasOne(x => x.InventoryItem)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemId);
        }

        private static void Configure(this EntityTypeBuilder<SparePartSellingInvoice> entity, string schema, string sequenceName)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(schema);

            entity.Property(x => x.InvoiceNumber)
                .HasDefaultValueSql($"nextval('\"{schema}\".\"{sequenceName}\"')");

            entity.Property(x => x.ClientName)
                .IsRequired()
                .HasMaxLength(300);

            entity.HasMany(x => x.SparePartSellingInvoiceLines)
                .WithOne(x => x.SellingInvoice)
                .HasForeignKey(x => x.SellingInvoiceId);

            entity.Property(x => x.IsVoided)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(x => x.VoidReason)
                .HasMaxLength(500);

            entity.Property(x => x.VoidedBy)
                .HasMaxLength(100);
        }

        private static void Configure(this EntityTypeBuilder<SparePartSellingInvoiceLine> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.HasOne(x => x.InventoryItem)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemId);
        }
    }
}
