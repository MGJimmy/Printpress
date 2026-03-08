using Printpress.Domain;

namespace Printpress.Application;

public interface IUnitOfWork
{
    IGenericRepository<Service> ServiceRepository { get; }
    IGenericRepository<Client> ClientRepository { get; }
    IGenericRepository<Order> OrderRepository { get; }
    IGenericRepository<OrderTransaction> OrderTransactionRepository { get; }
    IGenericRepository<OrderItemDetails> ItemDetailsRepository { get; }

    IGenericRepository<OrderGroup> OrderGroupRepository { get; }
    IInventoryItemRepository InventoryItemRepository { get; }
    IGenericRepository<InventoryTransaction> InventoryTransactionRepository { get; }
    IGenericRepository<PurchaseInvoice> PurchaseInvoiceRepository { get; }
    IGenericRepository<PurchaseInvoiceLine> PurchaseInvoiceLineRepository { get; }

    Task SaveChangesAsync(string userId);
}
