using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain.Entities;

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
    }
}
