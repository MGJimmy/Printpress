using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public int OrderGroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderGroup OrderGroup { get; set; }
    }
}
