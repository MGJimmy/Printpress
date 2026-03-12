using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain;

namespace Printpress.Infrastructure
{
    public static class HRConfigurations
    {
        private const string Schema = "HR";

        public static void ConfigureHR(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Worker>().Configure();
            modelBuilder.Entity<WorkerProduction>().Configure();
            modelBuilder.Entity<WorkerSalaryTransaction>().Configure();
            modelBuilder.Entity<PayrollPeriod>().Configure();
        }

        private static void Configure(this EntityTypeBuilder<Worker> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.PhoneNumber)
                .HasMaxLength(50);

            entity.Property(x => x.Address)
                .HasMaxLength(500);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.Property(x => x.SalaryType)
                .HasConversion<int>();

            entity.HasMany(x => x.SalaryTransactions)
                .WithOne(x => x.Worker)
                .HasForeignKey(x => x.WorkerId);

            entity.HasMany(x => x.WorkerProductions)
                .WithOne(x => x.Worker)
                .HasForeignKey(x => x.WorkerId);
        }

        private static void Configure(this EntityTypeBuilder<WorkerProduction> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.HasOne(x => x.OrderItem)
                .WithMany()
                .HasForeignKey(x => x.OrderItemId);

            entity.HasOne(x => x.ServiceCategory)
                .WithMany()
                .HasForeignKey(x => x.ServiceCategoryId);
        }

        private static void Configure(this EntityTypeBuilder<WorkerSalaryTransaction> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Note)
                .HasMaxLength(500);

            entity.Property(x => x.TransactionType)
                .HasConversion<int>();

            entity.HasOne(x => x.PayrollPeriod)
                .WithMany()
                .HasForeignKey(x => x.PayrollPeriodId);
        }

        private static void Configure(this EntityTypeBuilder<PayrollPeriod> entity)
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
