using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class ItemServiceExecution : Entity
    {
        public Guid WorkerId { get; set; }
        public Guid OrderItemId { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public virtual Worker Worker { get; set; }
        public virtual OrderItem OrderItem { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }
    }
}
