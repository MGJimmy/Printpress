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
                    StockQuantity = Context.InventoryTransaction
                        .Where(t => t.InventoryItemId == i.Id)
                        .Sum(t => t.InventoryTransactionType == InventoryTransactionType.In 
                            ? t.Quantity 
                            :- t.Quantity)
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
    }
}
