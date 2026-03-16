using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain;

namespace Printpress.Infrastructure
{
    public static class OrdersConfigurations
    {
        private const string Schema = "Orders";

        public static void ConfigureOrders(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<OrderTransaction>().Configure();
            modelBuilder.Entity<Service>().Configure();
            modelBuilder.Entity<ServiceCategory>().Configure();
            modelBuilder.Entity<Client>().Configure();
            modelBuilder.Entity<Order>().Configure();
            modelBuilder.Entity<OrderGroup>().Configure();
            modelBuilder.Entity<OrderItem>().Configure();
            modelBuilder.Entity<OrderItemDetails>().Configure();
            modelBuilder.Entity<ItemDetailsKey_LKP>().Configure();
            modelBuilder.Entity<OrderSellingItem>().Configure();


            modelBuilder.ApplyGlobalQueryFilters();
        }

        private static void ApplyGlobalQueryFilters(this ModelBuilder modelBuilder)
        {
            ExcludeSoftDeletedRowsFilter(modelBuilder);
        }

        private static void ExcludeSoftDeletedRowsFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Apply only to entities that implement ISoftDelete
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var prop = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                    var condition = Expression.Equal(prop, Expression.Constant(false));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        private static void Configure(this EntityTypeBuilder<OrderTransaction> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Notes)
                .HasMaxLength(500);
        }

        private static void Configure(this EntityTypeBuilder<Service> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Name)
                .HasMaxLength(200);

            entity.HasOne(x => x.ServiceCategory)
                 .WithMany()
                 .HasForeignKey(x => x.ServiceCategoryId);
        
            entity.HasOne(x => x.InventoryItem)
                    .WithMany(i => i.OrderServices)
                    .HasForeignKey(x => x.InventoryItemId);

            entity.Property(x => x.IsActive).HasDefaultValue(true);
        }

        private static void Configure(this EntityTypeBuilder<ServiceCategory> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(x => x.InventoryItemCategory_LKP)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemCategoryId);
        }

        private static void Configure(this EntityTypeBuilder<Client> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

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
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        }

        private static void Configure(this EntityTypeBuilder<OrderGroup> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.DeliveryName)
                .HasMaxLength(200);
            entity.Property(x => x.ReceiverName)
                .HasMaxLength(200);

            entity.Property(x => x.DeliveryNotes)
                .HasMaxLength(500);

            entity.Property(x => x.Status)
                .HasDefaultValue(GroupStatusEnum.New);
        }

        private static void Configure(this EntityTypeBuilder<OrderItem> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.OrderItemStatus)
                .HasConversion<int>()
                .HasDefaultValue(OrderItemStatus.New);
        }

        private static void Configure(this EntityTypeBuilder<OrderItemDetails> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

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
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }

        private static void Configure(this EntityTypeBuilder<OrderSellingItem> entity)
        {
            entity.SetSchemaTable(Schema);

            entity
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(x => x.Order)
                .WithMany(o => o.SellingItems)
                .HasForeignKey(x => x.OrderId);

            entity.HasOne(x => x.InventoryItem)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemId);

        }
    }
}
