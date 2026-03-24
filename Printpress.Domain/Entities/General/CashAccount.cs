using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class CashAccount 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}
