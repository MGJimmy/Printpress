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

        public List<InventoryTransaction> CreateInventoryTransaction(List<PurchaseInvoiceLine> purchaseInvoiceLines)
        {
            return purchaseInvoiceLines.Select(x =>
            {
                var transaction = new InventoryTransaction(
                    x.InventoryItemId,
                    InventoryTransactionType.In,
                    (int)x.Quantity,
                    InventoryTransactionReferenceType.Purchase,
                    x.Id,
                    $"Purchase of {x.Quantity} units at price {x.UnitPrice} per unit"
                );
                transaction.Id = _guidGenerator.NewGuid();
                return transaction;
            }).ToList();
        }

        public List<InventoryTransaction> CreatePurchaseVoidTransactions(List<PurchaseInvoiceLine> purchaseInvoiceLines, string invoiceNumber)
        {
            return purchaseInvoiceLines.Select(x =>
            {
                var transaction = new InventoryTransaction(
                    x.InventoryItemId,
                    InventoryTransactionType.Out,
                    (int)x.Quantity,
                    InventoryTransactionReferenceType.Purchase,
                    x.Id,
                    $"إلغاء فاتورة شراء {invoiceNumber}"
                );
                transaction.Id = _guidGenerator.NewGuid();
                return transaction;
            }).ToList();
        }
    }
}
