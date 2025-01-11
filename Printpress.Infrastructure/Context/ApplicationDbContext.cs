using Microsoft.EntityFrameworkCore;
using Printpress.Domain.Entities;

namespace Printpress.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Client> Client { get; set; }
    public DbSet<PrintingServiceDetails> PrintingServiceDetails { get; set; }
    public DbSet<PrintingType> PrintingType { get; set; }
    public DbSet<ProductStock> ProductStock { get; set; }
    public DbSet<Service> Service { get; set; }

    public DbSet<Order> Order { get; set; }
    public DbSet<OrderGroup> OrderGroup { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<ItemDetails> ItemDetails { get; set; }
    public DbSet<OrderGroupService> OrderGroupService { get; set; }
    public DbSet<OrderService> OrderService { get; set; }
    public DbSet<OrderTransaction> OrderTransaction { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureClient(modelBuilder);
        ConfigurePrintingServiceDetails(modelBuilder);
        ConfigurePrintingType(modelBuilder);
        ConfigureProductStock(modelBuilder);
        ConfigureService(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureOrderGroup(modelBuilder);
        ConfigureOrderTransaction(modelBuilder);
    }

    private void ConfigureOrderTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderTransaction>()
            .Property(x => x.Notes)
            .HasMaxLength(500);
    }

    private void ConfigureService(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>()
            .Property(x => x.Name)
            .HasMaxLength(200);
    }

    private void ConfigureProductStock(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductStock>()
            .Property(x => x.Name)
            .HasMaxLength(200);
    }

    private void ConfigurePrintingType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrintingType>()
            .Property(x => x.Name)
            .HasMaxLength(200);
    }

    private void ConfigurePrintingServiceDetails(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrintingServiceDetails>()
            .HasKey(x =>  x.ServiceId);
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



