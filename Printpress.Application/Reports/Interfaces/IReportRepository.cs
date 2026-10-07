namespace Printpress.Application;

public interface IReportRepository
{
    Task<InventoryItemReportData?> GetInventoryItemDataAsync(Guid inventoryItemId);
    Task<int> GetInventoryCartonsInAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<int> GetInventorycartonsOutAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<int> GetUsageSettlementUnitsAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive);
    Task<List<OrderItemUsageProjection>> GetOrderItemsUsageAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<int> GetDeliveredSellingCartonsAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive);
    Task<int> GetConversionUnitsAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive);

    // Report 2: Inventory & Services Usage
    Task<List<InventoryItemStockProjection>> GetInventoryItemsStockByCategoryAsync(int categoryId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<ServiceCategoryFilterDto>> GetAllServiceCategoriesAsync();
    Task<List<ServiceBasicInfo>> GetServicesByCategoryIdAsync(Guid serviceCategoryId);
    Task<Dictionary<Guid, int>> GetOrderCountsByServiceAsync(List<Guid> serviceIds, DateTime? dateFrom, DateTime? dateTo);
    Task<List<ServiceItemRaw>> GetServiceItemRawDataAsync(List<Guid> serviceIds, DateTime? dateFrom, DateTime? dateTo);

    Task<List<InventoryStockBalanceRowDto>> GetInventoryStockBalanceAsync(
        int? categoryId, DateTime? dateFrom, DateTime? dateToExclusive);

    Task<List<InventoryPurchaseLineRowDto>> GetInventoryPurchasesAsync(
        int? categoryId, Guid? inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive);

    Task<List<InventoryStockOutRowDto>> GetInventoryStockOutsAsync(
        int? categoryId, Guid? inventoryItemId, Guid? workerId, DateTime? dateFrom, DateTime? dateToExclusive);

    Task<List<InventoryMovementTxProjection>> GetInventoryMovementsAsync(Guid inventoryItemId);

    Task<List<OutstandingDocumentProjection>> GetOpenOrdersAsync();
    Task<List<OutstandingDocumentProjection>> GetUnpaidWorkerAdvancesAsync();
    Task<List<OutstandingDocumentProjection>> GetOpenPurchaseInvoicesAsync();
    Task<List<OutstandingDocumentProjection>> GetOpenSparePartPurchaseInvoicesAsync();
    Task<List<OutstandingDocumentProjection>> GetOpenLoansAsync();
    Task<List<OutstandingMonthlySalaryWorkerProjection>> GetActiveMonthlySalaryWorkersAsync();
    Task<List<OutstandingSalaryTransactionProjection>> GetSalaryTransactionsFromAsync(DateTime fromUtc, List<Guid> workerIds);
}
