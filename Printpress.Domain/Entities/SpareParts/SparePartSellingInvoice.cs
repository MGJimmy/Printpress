using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class SparePartSellingInvoice : Entity
    {
        public int InvoiceNumber { get; set; }
        public string ClientName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual ICollection<SparePartSellingInvoiceLine> SparePartSellingInvoiceLines { get; set; }
    }
}
