using Printpress.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities
{
    public class OrderGroup : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryName { get; set; }
        public string ReceiverName { get; set; }
        public OrderStatusEnum Status { get; set; }


        public virtual Client Client { get; set; }
    }
}
