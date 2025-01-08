using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities
{
    public class OrderGroupService : Entity
    {
        public int Id { get; set; }
        public int OrderGroupId { get; set; }
        public int ServiceId { get; set; }


        public OrderGroup OrderGroup { get; set; }
        public Service Service { get; set; }
    }
}
