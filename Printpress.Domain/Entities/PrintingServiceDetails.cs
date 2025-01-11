﻿using System;
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

        public virtual Service Service { get; set; }
        public virtual PrintingType PrintingType { get; set; }
        public virtual ProductStock ProductStock { get; set; }
    }
}
