using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartTransactionService(IUnitOfWork _unitOfWork, IMapper _mapper) : ISparePartTransactionService
{
    public async Task<PagedList<SparePartTransactionDto>> GetByItemIdAsync(
        Guid itemId,
        Paging paging,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        string transactionType)
    {
        var sorting = new Sorting(nameof(SparePartInventoryTransaction.CreatedAt), SortingDirection.DESC);

        var result = await _unitOfWork.SparePartTransactionRepository.FilterAsync(
            paging,
            x => x.InventoryItemId == itemId
                && (!dateFrom.HasValue || x.CreatedAt >= dateFrom.Value)
                && (!dateToExclusive.HasValue || x.CreatedAt < dateToExclusive.Value)
                && (string.IsNullOrEmpty(transactionType) || x.InventoryTransactionType.ToString() == transactionType),
            sorting);

        var movements = result.Items.ToList();
        var purchaseNumbers = await LoadPurchaseNumbersAsync(movements);
        var saleNumbers = await LoadSaleNumbersAsync(movements);

        var items = _mapper.Map<List<SparePartTransactionDto>>(movements);
        for (var i = 0; i < movements.Count; i++)
            items[i] = WithReference(items[i], movements[i], purchaseNumbers, saleNumbers);

        return new PagedList<SparePartTransactionDto>
        {
            Items = items,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    private async Task<Dictionary<Guid, int>> LoadPurchaseNumbersAsync(List<SparePartInventoryTransaction> movements)
    {
        var ids = ReferenceIds(movements, SparePartInventoryTransactionReferenceType.Purchase);
        if (ids.Count == 0)
            return [];

        var invoices = await _unitOfWork.SparePartPurchaseInvoiceRepository.FilterAsync(i => ids.Contains(i.Id));
        return invoices.ToDictionary(i => i.Id, i => i.InvoiceNumber);
    }

    private async Task<Dictionary<Guid, int>> LoadSaleNumbersAsync(List<SparePartInventoryTransaction> movements)
    {
        var ids = ReferenceIds(movements, SparePartInventoryTransactionReferenceType.Sale);
        if (ids.Count == 0)
            return [];

        var invoices = await _unitOfWork.SparePartSellingInvoiceRepository.FilterAsync(i => ids.Contains(i.Id));
        return invoices.ToDictionary(i => i.Id, i => i.InvoiceNumber);
    }

    private static List<Guid> ReferenceIds(
        List<SparePartInventoryTransaction> movements,
        SparePartInventoryTransactionReferenceType referenceType)
    {
        return movements
            .Where(movement => movement.ReferenceType == referenceType && movement.ReferenceId.HasValue)
            .Select(movement => movement.ReferenceId.Value)
            .Distinct()
            .ToList();
    }

    private static SparePartTransactionDto WithReference(
        SparePartTransactionDto dto,
        SparePartInventoryTransaction movement,
        Dictionary<Guid, int> purchaseNumbers,
        Dictionary<Guid, int> saleNumbers)
    {
        if (movement.ReferenceType == SparePartInventoryTransactionReferenceType.Purchase && movement.ReferenceId is Guid purchaseId)
        {
            var number = purchaseNumbers.TryGetValue(purchaseId, out var purchaseNumber) ? purchaseNumber.ToString() : "—";
            return dto with
            {
                ReferenceLabel = $"فاتورة شراء: {number}",
                ReferenceRoute = $"/spare-parts/stock-in/invoices/{purchaseId}"
            };
        }

        if (movement.ReferenceType == SparePartInventoryTransactionReferenceType.Sale && movement.ReferenceId is Guid saleId)
        {
            var number = saleNumbers.TryGetValue(saleId, out var saleNumber) ? saleNumber.ToString() : "—";
            return dto with
            {
                ReferenceLabel = $"فاتورة بيع: {number}",
                ReferenceRoute = $"/spare-parts/stock-out/invoices/{saleId}"
            };
        }

        return dto with
        {
            ReferenceLabel = "—",
            ReferenceRoute = null
        };
    }
}
