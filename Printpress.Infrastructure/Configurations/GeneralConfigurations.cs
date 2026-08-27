using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain;

namespace Printpress.Infrastructure
{
    public static class GeneralConfigurations
    {
        private const string Schema = "General";

        public static void ConfigureGeneral(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CashAccount>().Configure();
            modelBuilder.Entity<CashTransaction>().Configure();
        }

        private static void Configure(this EntityTypeBuilder<CashAccount> entity)
        {
            entity.SetSchemaTable(Schema);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Balance)
                .HasPrecision(18, 4);

            entity.UseXminAsConcurrencyToken();
            entity.Navigation(x => x.Transactions).UsePropertyAccessMode(PropertyAccessMode.Field);
        }

        private static void Configure(this EntityTypeBuilder<CashTransaction> entity)
        {
            entity.SetSchemaTable(Schema);

            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 4);

            entity.Property(x => x.TransactionDate)
                .IsRequired();

            entity.Property(x => x.Type)
                .HasConversion<int>();

            entity.Property(x => x.Category)
                .HasConversion<int>();

            entity.Property(x => x.ReferenceType)
                .HasConversion<int?>();

            entity.Property(x => x.IsVoided)
                .IsRequired();

            entity.HasOne(x => x.ReversesTransaction)
                .WithMany()
                .HasForeignKey(x => x.ReversesTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CashAccount)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.CashAccountId);
        }
    }
}
