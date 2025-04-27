using Microsoft.EntityFrameworkCore;
using Printpress.Domain.Entities;
using Printpress.Domain.Interfaces;

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



    public DbSet<ItemDetailsKey_LKP> ItemDetailsKey_LKP { get; set; }
    public DbSet<ServiceCategory_LKP> ServiceCategory_LKP { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Configure();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        HandleSoftDelete();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleSoftDelete();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void HandleSoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ISoftDelete softDeleteEntity && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDeleteEntity.IsDeleted = true;
            }
        }
    }

}



