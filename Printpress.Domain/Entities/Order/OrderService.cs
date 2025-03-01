﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities
{
    public class OrderService : Entity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public decimal? Price { get; set; }


        public virtual Order Order { get; set; }
        public virtual Service Service { get; set; }
    }
}
