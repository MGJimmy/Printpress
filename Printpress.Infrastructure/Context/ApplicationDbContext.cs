using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        modelBuilder.Configure();
    }




}



