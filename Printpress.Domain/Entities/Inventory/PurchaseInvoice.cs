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
        public decimal PaidAmount { get; private set; }
        public bool IsGoodsReceived { get; private set; }
        public string AttachmentFilePath { get; private set; }
        public bool IsVoided { get; private set; }
        public string VoidReason { get; private set; }
        public DateTime? VoidedAt { get; private set; }
        public string VoidedBy { get; private set; }

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

        public void SetInitialSettlement(decimal paidAmount, bool isGoodsReceived)
        {
            PaidAmount = paidAmount;
            IsGoodsReceived = isGoodsReceived;
        }

        public void ApplyPayment(decimal amount)
        {
            if (IsVoided)
                throw new BusinessExceptions(LocalizationKeys.Invoices.AlreadyVoided);
            if (amount <= 0)
                throw new BusinessExceptions(LocalizationKeys.Invoices.PaymentAmountInvalid);
            if (PaidAmount + amount > TotalAmount)
                throw new BusinessExceptions(LocalizationKeys.Invoices.PaymentExceedsRemaining);

            PaidAmount += amount;
        }

        public void ReceiveGoods()
        {
            if (IsVoided)
                throw new BusinessExceptions(LocalizationKeys.Invoices.AlreadyVoided);
            if (IsGoodsReceived)
                throw new BusinessExceptions(LocalizationKeys.Invoices.AlreadyReceived);

            IsGoodsReceived = true;
        }

        public void MarkAsVoided(string reason, string userId)
        {
            if (IsVoided)
                throw new BusinessExceptions(LocalizationKeys.Invoices.AlreadyVoided);

            IsVoided = true;
            VoidReason = reason;
            VoidedAt = DateTime.UtcNow;
            VoidedBy = userId;
            PaidAmount = 0;
        }
    }
}
