using Printpress.Domain;

namespace Printpress.Application;

public interface IUnitOfWork
{
    IGenericRepository<Service> ServiceRepository { get; }
    IGenericRepository<ServiceCategory> ServiceCategoryRepository { get; }
    IGenericRepository<Client> ClientRepository { get; }
    IGenericRepository<Order> OrderRepository { get; }
    IGenericRepository<OrderTransaction> OrderTransactionRepository { get; }
    IGenericRepository<OrderItemDetails> ItemDetailsRepository { get; }

    IGenericRepository<OrderGroup> OrderGroupRepository { get; }
    IInventoryItemRepository InventoryItemRepository { get; }
    IGenericRepository<InventoryTransaction> InventoryTransactionRepository { get; }
    IGenericRepository<PurchaseInvoice> PurchaseInvoiceRepository { get; }
    IGenericRepository<PurchaseInvoiceLine> PurchaseInvoiceLineRepository { get; }

    ISparePartItemRepository SparePartItemRepository { get; }
    IGenericRepository<SparePartInventoryTransaction> SparePartTransactionRepository { get; }
    IGenericRepository<SparePartPurchaseInvoice> SparePartPurchaseInvoiceRepository { get; }
    IGenericRepository<SparePartSellingInvoice> SparePartSellingInvoiceRepository { get; }

    IReportRepository ReportRepository { get; }

    IGenericRepository<Worker> WorkerRepository { get; }
    IGenericRepository<ItemServiceExecution> WorkerProductionRepository { get; }
    IGenericRepository<WorkerSalaryTransaction> WorkerSalaryTransactionRepository { get; }
    IGenericRepository<PayrollPeriod> PayrollPeriodRepository { get; }

    Task SaveChangesAsync(string userId);
}
