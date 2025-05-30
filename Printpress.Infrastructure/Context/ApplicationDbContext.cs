using Microsoft.EntityFrameworkCore;
using Printpress.Domain.Entities;
using Printpress.Domain.Interfaces;

namespace Printpress.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public string CurrentUserId { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Configure();


    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyCustomSaveLogic();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyCustomSaveLogic();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyCustomSaveLogic();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyCustomSaveLogic();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyCustomSaveLogic()
    {
        HandleSoftDelete();
        SetAuditFields();
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

    private void SetAuditFields()
    {
        // Only check for user ID if there are added or modified entities
        var auditableAddedUpdatedEntries = ChangeTracker.Entries<IAuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

        if (auditableAddedUpdatedEntries.Any() && string.IsNullOrEmpty(CurrentUserId))
        {
            throw new InvalidOperationException("CurrentUserId must be set before saving changes.");
        }

        var utcDateNow = DateTime.UtcNow;
        foreach (var entry in auditableAddedUpdatedEntries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = CurrentUserId;
                    entry.Entity.CreatedAt = utcDateNow;
                    entry.Entity.UpdatedBy = CurrentUserId;
                    entry.Entity.UpdatedAt = utcDateNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedBy = CurrentUserId;
                    entry.Entity.UpdatedAt = utcDateNow;
                    // Prevent accidental changes to CreatedBy/CreatedAt
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;
            }
        }
    }
}



