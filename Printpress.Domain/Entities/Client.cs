﻿namespace Printpress.Domain.Entities
{
    public class Client : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }

        public virtual List<Order> Orders { get; set; }
    }
}
