using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Application
{
    public class OrderServiceUpsertDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public decimal? Price { get; set; }
    }
}
