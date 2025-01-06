using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities
{
    public class PrintingServiceDetails : Entity
    {
        public int ServiceId { get; set; }

        public int PrintingTypeId { get; set; }

        public int ProductStockId { get; set; }

        public Service Service { get; set; }
        public PrintingType PrintingType { get; set; }
        public ProductStock ProductStock { get; set; }
    }
}
