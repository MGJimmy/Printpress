using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class InventoryTransactionDomainService : IInventoryTransactionDomainService
    {
        private readonly IGuidGenerator _guidGenerator;

        public InventoryTransactionDomainService(IGuidGenerator guidGenerator)
        {
            _guidGenerator = guidGenerator;
        }

        public List<InventoryTransaction> CreateInventoryTransaction(List<PurchaseInvoiceLine> purchaseInvoiceLines, DateTime occurredAt, string notes)
        {
            return purchaseInvoiceLines.Select(x =>
            {
                var movementNotes = string.IsNullOrWhiteSpace(notes)
                    ? $"Purchase of {x.Quantity} units at price {x.UnitPrice} per unit"
                    : notes;
                var transaction = new InventoryTransaction(
                    x.InventoryItemId,
                    InventoryTransactionType.In,
                    (int)x.Quantity,
                    InventoryTransactionReferenceType.Purchase,
                    x.Id,
                    movementNotes,
                    occurredAt
                );
                transaction.Id = _guidGenerator.NewGuid();
                return transaction;
            }).ToList();
        }

        public List<InventoryTransaction> CreatePurchaseVoidTransactions(List<PurchaseInvoiceLine> purchaseInvoiceLines, int invoiceNumber, DateTime occurredAt)
        {
            return purchaseInvoiceLines.Select(x =>
            {
                var transaction = new InventoryTransaction(
                    x.InventoryItemId,
                    InventoryTransactionType.Out,
                    (int)x.Quantity,
                    InventoryTransactionReferenceType.Purchase,
                    x.Id,
                    $"إلغاء فاتورة شراء {invoiceNumber}",
                    occurredAt
                );
                transaction.Id = _guidGenerator.NewGuid();
                return transaction;
            }).ToList();
        }
    }
}
