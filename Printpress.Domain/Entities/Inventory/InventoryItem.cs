using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain.Enums;

namespace Printpress.Domain.Entities
{
    public class InventoryItem : Entity
    {
        public int Id { get; set; }
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
    }
}
