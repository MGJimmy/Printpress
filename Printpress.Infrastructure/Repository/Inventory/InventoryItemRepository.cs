using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Printpress.Application;
using Printpress.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Printpress.Infrastructure
{
    internal class InventoryItemRepository : GenericRepository<InventoryItem>, IInventoryItemRepository
    {
        public InventoryItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<PagedList<InventoryItemDto>> GetAllWithStockQuantity(Paging paging)
        {
            var items = Context.InventoryItem
                .Select(i => new InventoryItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    InventoryItemCategory = i.InventoryItemCategory,
                    PacksPerCarton = i.PacksPerCarton,
                    UnitsPerPack = i.UnitsPerPack,
                    ExpectedPurchaseLossPercent = i.ExpectedPurchaseLossPercent,
                    ExpectedProductionWastePercent = i.ExpectedProductionWastePercent,
                    HasTransactions = i.InventoryTransactions.Any(),
                    IsActive = i.IsActive,
                    StockQuantity = Context.InventoryTransaction
                        .Where(t => t.InventoryItemId == i.Id)
                        .Sum(t => t.InventoryTransactionType == InventoryTransactionType.In
                            ? t.Quantity
                            : -t.Quantity)
                })
                .SelectPage(paging);

            return new PagedList<InventoryItemDto>
            {
                Items = await items.ToListAsync(),
                TotalCount = Context.InventoryItem.Count(),
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task<InventoryItemDto?> FindByIdWithStockQuantity(Guid id)
        {
            return await Context.InventoryItem
                .Where(i => i.Id == id)
                .Select(i => new InventoryItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    InventoryItemCategory = i.InventoryItemCategory,
                    PacksPerCarton = i.PacksPerCarton,
                    UnitsPerPack = i.UnitsPerPack,
                    ExpectedPurchaseLossPercent = i.ExpectedPurchaseLossPercent,
                    ExpectedProductionWastePercent = i.ExpectedProductionWastePercent,
                    HasTransactions = i.InventoryTransactions.Any(),
                    IsActive = i.IsActive,
                    StockQuantity = Context.InventoryTransaction
                        .Where(t => t.InventoryItemId == i.Id)
                        .Sum(t => t.InventoryTransactionType == InventoryTransactionType.In
                            ? t.Quantity
                            : -t.Quantity)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<InventoryItemDto>> GetByCategoryIdAsync(int categoryId)
        {
            return await Context.InventoryItem
                .Where(i => i.InventoryItemCategoryId == categoryId && i.IsActive)
                .Select(i => new InventoryItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    InventoryItemCategory = i.InventoryItemCategory,
                    PacksPerCarton = i.PacksPerCarton,
                    UnitsPerPack = i.UnitsPerPack,
                    ExpectedPurchaseLossPercent = i.ExpectedPurchaseLossPercent,
                    ExpectedProductionWastePercent = i.ExpectedProductionWastePercent,
                    HasTransactions = i.InventoryTransactions.Any(),
                    IsActive = i.IsActive,
                    StockQuantity = Context.InventoryTransaction
                        .Where(t => t.InventoryItemId == i.Id)
                        .Sum(t => t.InventoryTransactionType == InventoryTransactionType.In
                            ? t.Quantity
                            : -t.Quantity)
                })
                .ToListAsync();
        }


        public async Task<List<EnumBasicInfoDto>> GetInventoryCategoriesAllAsync()
        {
            return await Context.InventoryItemCategory_LKP
                .Select(g => new EnumBasicInfoDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();
        }

        public async Task<List<EnumBasicInfoDto>> GetServiceCategoriesAsync()
        {
            return await Context.ServiceCategory
                .Where(sc => sc.InventoryItemCategoryId != null && sc.InventoryItemCategory_LKP != null)
                .GroupBy(sc => sc.InventoryItemCategoryId!.Value)
                .Select(g => new EnumBasicInfoDto
                {
                    Id = g.Key,
                    Name = g.First().InventoryItemCategory_LKP.Name
                })
                .ToListAsync();
        }

        public async Task<List<EntityBasicInfoDto>> GetInventoryItemsForReportAsync(int categoryId)
        {
            var linkedItemIds = await Context.Service
                .Where(s => s.InventoryItemId != null)
                .Select(s => s.InventoryItemId!.Value)
                .Distinct()
                .ToListAsync();

            return await Context.InventoryItem
                .Where(i => i.InventoryItemCategoryId == categoryId && linkedItemIds.Contains(i.Id))
                .Select(i => new EntityBasicInfoDto { Id = i.Id, Name = i.Name })
                .ToListAsync();
        }
    }
}
