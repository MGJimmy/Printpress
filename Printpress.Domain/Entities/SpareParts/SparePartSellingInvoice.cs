using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class SparePartSellingInvoice : Entity
    {
        public List<SparePartSellingInvoiceLine> _sparePartSellingInvoiceLines = new();

        public int InvoiceNumber { get; private set; }
        public string ClientName { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsVoided { get; private set; }
        public string VoidReason { get; private set; }
        public DateTime? VoidedAt { get; private set; }
        public string VoidedBy { get; private set; }

        public IReadOnlyCollection<SparePartSellingInvoiceLine> SparePartSellingInvoiceLines => _sparePartSellingInvoiceLines.AsReadOnly();

        private SparePartSellingInvoice()
        {
        }

        public SparePartSellingInvoice(
            int invoiceNumber,
            string clientName,
            DateTime invoiceDate
            )
        {
            Id = Guid.NewGuid();
            InvoiceNumber = invoiceNumber;
            ClientName = clientName;
            InvoiceDate = invoiceDate;
        }

        public void AddLine(
            Guid inventoryItemId,
            decimal quantity,
            decimal unitPrice)
        {
            var lineTotalCalculated = quantity * unitPrice;

            var line = new SparePartSellingInvoiceLine
            {
                Id = Guid.NewGuid(),
                SellingInvoiceId = this.Id,
                InventoryItemId = inventoryItemId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotalCalculated
            };
            _sparePartSellingInvoiceLines.Add(line);
            TotalAmount += lineTotalCalculated;
        }

        public void MarkAsVoided(string reason, string userId)
        {
            if (IsVoided)
                throw new BusinessExceptions(LocalizationKeys.Invoices.AlreadyVoided);

            IsVoided = true;
            VoidReason = reason;
            VoidedAt = DateTime.UtcNow;
            VoidedBy = userId;
        }
    }
}
