using Printpress.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities
{
    public class Order : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ClientId { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalPaid { get; set; }
        public OrderStatusEnum Status { get; set; }


        public virtual Client Client { get; set; }
    }
}
