using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class OrderSellingItem : Entity
    {
        public string Name { get; set; }
        public Guid OrderId { get; set; }

        public Guid? InventoryItemId { get; set; }
        public bool IsInventoryItem { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
    }
}
