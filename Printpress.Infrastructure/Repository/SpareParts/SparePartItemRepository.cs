using Microsoft.EntityFrameworkCore;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.Infrastructure;

internal class SparePartItemRepository : GenericRepository<SparePartInventoryItem>, ISparePartItemRepository
{
    public SparePartItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<PagedList<SparePartItemDto>> GetAllWithStockQuantityAsync(Paging paging)
    {
        var query = Context.SparePartInventoryItem
            .Select(i => new SparePartItemDto
            {
                Id = i.Id,
                Name = i.Name,
                PacksPerCarton = i.PacksPerCarton,
                UnitsPerPack = i.UnitsPerPack,
                HasTransactions = i.InventoryTransactions.Any(),
                StockQuantity = Context.SparePartInventoryTransaction
                    .Where(t => t.InventoryItemId == i.Id)
                    .Sum(t => t.InventoryTransactionType == SparePartInventoryTransactionType.In
                        ? t.Quantity
                        : -t.Quantity)
            })
            .SelectPage(paging);

        return new PagedList<SparePartItemDto>
        {
            Items = await query.ToListAsync(),
            TotalCount = Context.SparePartInventoryItem.Count(),
            PageNumber = paging.PageNumber,
            PageSize = paging.PageSize
        };
    }

    public async Task<SparePartItemDto?> FindByIdWithStockQuantityAsync(Guid id)
    {
        return await Context.SparePartInventoryItem
            .Where(i => i.Id == id)
            .Select(i => new SparePartItemDto
            {
                Id = i.Id,
                Name = i.Name,
                PacksPerCarton = i.PacksPerCarton,
                UnitsPerPack = i.UnitsPerPack,
                HasTransactions = i.InventoryTransactions.Any(),
                StockQuantity = Context.SparePartInventoryTransaction
                    .Where(t => t.InventoryItemId == i.Id)
                    .Sum(t => t.InventoryTransactionType == SparePartInventoryTransactionType.In
                        ? t.Quantity
                        : -t.Quantity)
            })
            .FirstOrDefaultAsync();
    }
}
