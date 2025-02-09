using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Configure(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderTransaction>().Configure();
            modelBuilder.Entity<Service>().Configure();
            modelBuilder.Entity<ProductStock>().Configure();
            modelBuilder.Entity<PrintingType>().Configure();
            modelBuilder.Entity<PrintingServiceDetails>().Configure();
            modelBuilder.Entity<Client>().Configure();
            modelBuilder.Entity<Order>().Configure();
            modelBuilder.Entity<OrderGroup>().Configure();
            modelBuilder.Entity<Item>().Configure();
            modelBuilder.Entity<ItemDetails>().Configure();
            modelBuilder.Entity<ItemDetailsKey_LKP>().Configure();
            modelBuilder.Entity<ServiceCategory_LKP>().Configure();
        }

        private static void Configure(this EntityTypeBuilder<OrderTransaction> entity)
        {
            entity
                .Property(x => x.Notes)
                .HasMaxLength(500);
        }

        private static void Configure(this EntityTypeBuilder<Service> entity)
        {
            entity
                .Property(x => x.Name)
                .HasMaxLength(200);

            entity.HasOne(x => x.ServiceCategory_LKP)
                 .WithMany()
                 .HasForeignKey(x => x.ServiceCategoryId);

            entity.Ignore(x => x.ServiceCategory);
        }

        private static void Configure(this EntityTypeBuilder<ProductStock> entity)
        {
            entity
                .Property(x => x.Name)
                .HasMaxLength(200);
        }

        private static void Configure(this EntityTypeBuilder<PrintingType> entity)
        {
            entity
                .Property(x => x.Name)
                .HasMaxLength(200);
        }

        private static void Configure(this EntityTypeBuilder<PrintingServiceDetails> entity)
        {
            entity
                .HasKey(x => x.ServiceId);
        }

        private static void Configure(this EntityTypeBuilder<Client> entity)
        {
            entity
                .Property(x => x.Name)
                .HasMaxLength(200);

            entity
                .Property(x => x.Mobile)
                .HasMaxLength(20);

            entity
                .Property(x => x.Address)
                .HasMaxLength(500);
        }

        private static void Configure(this EntityTypeBuilder<Order> entity)
        {
            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        }

        private static void Configure(this EntityTypeBuilder<OrderGroup> entity)
        {
            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        }

        private static void Configure(this EntityTypeBuilder<Item> entity)
        {
            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        }

        private static void Configure(this EntityTypeBuilder<ItemDetails> entity)
        {
            entity
                .Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(x => x.ItemDetailsKey_LKP)
                .WithMany()
                .HasForeignKey(x => x.ItemDetailsKeyId);

            entity.Ignore(x => x.ItemDetailsKey);
        }

        private static void Configure(this EntityTypeBuilder<ItemDetailsKey_LKP> entity)
        {
            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }

        private static void Configure(this EntityTypeBuilder<ServiceCategory_LKP> entity)
        {
            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
