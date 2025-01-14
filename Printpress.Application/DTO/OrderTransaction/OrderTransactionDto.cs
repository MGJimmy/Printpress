using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    public class OrderTransactionDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
