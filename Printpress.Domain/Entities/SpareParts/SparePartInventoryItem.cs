using System;

namespace Printpress.Domain
{
    public class SparePartInventoryItem : Entity
    {
        public string Name { get; set; }
        public int? PacksPerCarton { get; set; }
        public int? UnitsPerPack { get; set; }

        public virtual ICollection<SparePartInventoryTransaction> InventoryTransactions { get; set; }
    }
}
