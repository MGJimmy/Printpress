using System;

namespace Printpress.Domain
{
    public class InventoryItem : Entity
    {
        public string Name { get; set; }
        public InventoryItemCategoryEnum InventoryItemCategory
        {
            get
            {
                return (InventoryItemCategoryEnum)InventoryItemCategoryId;
            }
            set
            {
                InventoryItemCategoryId = (int)value;
            }
        }
        public int InventoryItemCategoryId { get; set; }
        public int? PacksPerCarton { get; set; }
        public int? UnitsPerPack { get; set; }
        public int ExpectedPurchaseLossPercent { get; set; }
        public int ExpectedProductionWastePercent { get; set; }

        public virtual InventoryItemCategory_LKP InventoryItemCategory_LKP { get; set; }

        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }

        public virtual ICollection<Service> OrderServices { get; set; }
    }
}
