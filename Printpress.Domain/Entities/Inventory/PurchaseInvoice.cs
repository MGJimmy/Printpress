using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class PurchaseInvoice : Entity
    {
        private readonly List<PurchaseInvoiceLine> _purchaseInvoiceLines = new();

        public string InvoiceNumber { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public string SupplierName { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string AttachmentFilePath { get; private set; }

        public virtual IReadOnlyCollection<PurchaseInvoiceLine> PurchaseInvoiceLines
                => _purchaseInvoiceLines.AsReadOnly();

        private PurchaseInvoice()
        {
        
        }

        public PurchaseInvoice(
            string invoiceNumber, 
            DateTime invoiceDate, 
            string suplierName, 
            string attachmentFilePath
            )
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            SupplierName = suplierName;
            AttachmentFilePath = attachmentFilePath;

        }

        public void AddLine(Guid lineId, Guid inventoryItemId, decimal quantity, decimal unitPrice)
        {
            var lineTotal = quantity * unitPrice;
            var line = new PurchaseInvoiceLine
            {
                Id = lineId,
                PurchaseInvoiceId = this.Id,
                InventoryItemId = inventoryItemId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal
            };
            _purchaseInvoiceLines.Add(line);
            TotalAmount += lineTotal;
        }
    }
}
