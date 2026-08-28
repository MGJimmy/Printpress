using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities.Inventory.DomainServices
{
    public static class InventoryCalculatorDS
    {
        public static int CalculateStockQuantity(
            IEnumerable<InventoryTransaction> transactions)
        {
            return (transactions ?? []).Sum(t =>
                t.InventoryTransactionType == InventoryTransactionType.In
                    ? t.Quantity
                    : -t.Quantity);
        }

        public static int CalculateInQuantity(IEnumerable<InventoryTransaction> transactions)
        {
            return (transactions ?? [])
                .Where(t => t.InventoryTransactionType == InventoryTransactionType.In)
                .Sum(t => t.Quantity);
        }

        public static int CalculateOutQuantity(IEnumerable<InventoryTransaction> transactions)
        {
            return (transactions ?? [])
                .Where(t => t.InventoryTransactionType != InventoryTransactionType.In)
                .Sum(t => t.Quantity);
        }

        public static int CalculateStockUnits(
            int stockQuantity,
            int? packsPerCarton,
            int? unitsPerPack)
        {
            return stockQuantity
                * (packsPerCarton ?? 1)
                * (unitsPerPack ?? 1);
        }
    }
}
