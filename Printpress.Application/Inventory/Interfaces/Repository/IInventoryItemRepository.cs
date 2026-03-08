using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain;

namespace Printpress.Application
{
    public interface IInventoryItemRepository : IGenericRepository<InventoryItem>
    {
        Task<PagedList<InventoryItemDto>> GetAllWithStockQuantity(Paging paging);
        Task<InventoryItemDto?> FindByIdWithStockQuantity(Guid id);
    }
}
