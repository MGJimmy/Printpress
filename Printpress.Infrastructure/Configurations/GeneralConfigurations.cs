using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Printpress.Domain;

namespace Printpress.Infrastructure
{
    public static class GeneralConfigurations
    {
        private const string Schema = "General";
        private const string LoanNumberSequence = "LoanNumber";

        public static void ConfigureGeneral(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>(LoanNumberSequence, Schema).StartsAt(1).IncrementsBy(1);

            modelBuilder.Entity<CashAccount>().Configure();
            modelBuilder.Entity<CashTransaction>().Configure();
            modelBuilder.Entity<Lender>().Configure();
            modelBuilder.Entity<Loan>().Configure(Schema, LoanNumberSequence);
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

        private static void Configure(this EntityTypeBuilder<Lender> entity)
        {
            entity.SetSchemaTable(Schema);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Phone)
                .HasMaxLength(50);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);
        }

        private static void Configure(this EntityTypeBuilder<Loan> entity, string schema, string sequenceName)
        {
            entity.SetSchemaTable(Schema);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.LoanNumber)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql($"nextval('\"{schema}\".\"{sequenceName}\"')");

            entity.Property(x => x.Principal)
                .HasPrecision(18, 4);

            entity.Property(x => x.PaidAmount)
                .HasPrecision(18, 4)
                .HasDefaultValue(0m);

            entity.Property(x => x.OccurredAt)
                .IsRequired();

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.Property(x => x.VoidReason)
                .HasMaxLength(500);

            entity.Property(x => x.VoidedBy)
                .HasMaxLength(100);

            entity.Ignore(x => x.Remaining);
            entity.Ignore(x => x.IsClosed);

            entity.HasOne(x => x.Lender)
                .WithMany()
                .HasForeignKey(x => x.LenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<CashAccount>()
                .WithMany()
                .HasForeignKey(x => x.CashAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
