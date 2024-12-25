using Microsoft.EntityFrameworkCore;
using Printpress.Domain.Entities;

namespace Printpress.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureClient(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureOrderGroup(modelBuilder);
    }

    private void ConfigureClient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .Property(x => x.Name)
            .HasMaxLength(200);


        modelBuilder.Entity<Client>()
            .Property(x => x.Mobile)
            .HasMaxLength(20);

        modelBuilder.Entity<Client>()
            .Property(x => x.Address)
            .HasMaxLength(500);
    }

    private void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

    }

    private void ConfigureOrderGroup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderGroup>()
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

    }
}



