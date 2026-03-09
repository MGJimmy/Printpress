using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class OrderSellingItem : Entity
    {
        public string Name { get; private set; }
        public Guid OrderId { get; private set; }

        public Guid? InventoryItemId { get; private set; }
        public bool IsInventoryItem { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        public virtual Order Order { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
    }
}
