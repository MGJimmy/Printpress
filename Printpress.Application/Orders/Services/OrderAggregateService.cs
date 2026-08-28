using Microsoft.EntityFrameworkCore;
using Printpress.Domain;
namespace Printpress.Application;

internal sealed class OrderAggregateService(IUnitOfWork _IUnitOfWork, OrderMapper _OrderMapper, IGuidGenerator _guidGenerator, ILocalizationService _loc) : IOrderAggregateService
{
    public async Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize)
    {
        string[] includes = { nameof(Order.Client) };

        var orders = await _IUnitOfWork.OrderRepository.AllAsync(
            new Paging(pageNumber, pageSize),
            new Sorting(nameof(Order.Id), SortingDirection.DESC),
            includes
        );

        return _OrderMapper.MapToOrderSummeryDto(orders);
    }

    public async Task<OrderDto> GetOrderDTOAsync(Guid orderId)
    {
        string[] includes = [
            $"{nameof(Order.OrderGroups)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}.{nameof(OrderItem.Details)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}.{nameof(OrderGroupService.Service)}",
            $"{nameof(Order.Services)}",
            $"{nameof(Order.SellingItems)}",
            $"{nameof(Order.SellingItems)}.{nameof(OrderSellingItem.InventoryItem)}",
            $"{nameof(Order.Client)}"];

        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync((order => order.Id == orderId), false, includes);

        if (order is null) ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        var orderDTO = order.MapToOrderDTO();
        ApplyItemExecutionFlags(orderDTO, order);

        return orderDTO;
    }

    public async Task<OrderMainDataDto> GetOrderMainDataAsync(Guid orderId)
    {
        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync((order => order.Id == orderId), false, nameof(Order.Client));

        if (order is null) ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        return order.MapToOrderMainDataDto();
    }
    public async Task InsertOrder(OrderUpsertDto orderDTO, string userId)
    {

        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);

        order.Id = _guidGenerator.NewGuid();
        order.Status = OrderStatusEnum.New;
        order.TotalPaid = 0;

        foreach (var group in order.OrderGroups ?? [])
        {
            group.Id = _guidGenerator.NewGuid();
            group.OrderId = order.Id;
            foreach (var item in group.Items ?? [])
            {
                item.Id = _guidGenerator.NewGuid();
                item.OrderGroupId = group.Id;
                foreach (var detail in item.Details ?? [])
                    detail.Id = _guidGenerator.NewGuid();
            }
            foreach (var gs in group.OrderGroupServices ?? [])
                gs.Id = _guidGenerator.NewGuid();
        }
        foreach (var os in order.Services ?? [])
            os.Id = _guidGenerator.NewGuid();

        foreach (var si in order.SellingItems ?? [])
        {
            si.Id = _guidGenerator.NewGuid();
            si.OrderId = order.Id;
        }

        ApplyZeroOrderFlag(order);
        ApplyZeroOrderPrices(order);
        order.TotalPrice = await CalculateOrderTotalPrice(order);

        await _IUnitOfWork.OrderRepository.AddAsync(order);

        await _IUnitOfWork.SaveChangesAsync(userId);
    }

    private static void ApplyZeroOrderFlag(Order order)
    {
        var services = (order.Services ?? []).NotDeleted().ToList();
        if (services.Count == 0)
            return;

        order.IsZeroOrder = services.All(s => s.Price.GetValueOrDefault() <= 0);
    }

    private static void ApplyZeroOrderPrices(Order order)
    {
        if (!order.IsZeroOrder)
            return;

        foreach (var service in (order.Services ?? []).NotDeleted())
            service.Price = 0;

        foreach (var item in (order.SellingItems ?? []).NotDeleted())
            item.Price = 0;
    }

    private async Task<decimal?> CalculateOrderTotalPrice(Order order)
    {
        decimal totalOrderPrice = 0;

        foreach (var group in order.OrderGroups.NotDeleted())
        {
            await SetGroupItemPrices(group, order.Services);

            totalOrderPrice += group.Items.NotDeleted().Sum(i => i.Price * i.Quantity);
        }

        totalOrderPrice += (order.SellingItems ?? []).NotDeleted().Sum(i => i.Price * i.Quantity);

        return totalOrderPrice;
    }

    private async Task SetGroupItemPrices(OrderGroup group, List<OrderService> orderService)
    {
        var allServices = await _IUnitOfWork.ServiceRepository.AllAsync(nameof(Service.ServiceCategory));

        var activeGroupServices = group.OrderGroupServices.NotDeleted().ToList();
        var groupServicesIds = new HashSet<Guid>(activeGroupServices.Select(d => d.ServiceId));
        var currentGroupServices = allServices.Where(s => groupServicesIds.Contains(s.Id)).ToList();

        if (currentGroupServices.Exists(x => x.ServiceCategory?.Code == "Selling"))
        {
            return;
        }

        ValidateGroupServices(activeGroupServices, currentGroupServices);

        var printingGroupServices = activeGroupServices
            .Where(gs => currentGroupServices.Any(s => s.Id == gs.ServiceId && s.ServiceCategory?.Code == "Printing"))
            .ToList();
        var staplingService = currentGroupServices.Find(x => x.ServiceCategory?.Code == "Stapling");
        var cluingService = currentGroupServices.Find(x => x.ServiceCategory?.Code == "Clueing");
        var cuttingService = currentGroupServices.Find(x => x.ServiceCategory?.Code == "Cutting");

        foreach (var item in group.Items.NotDeleted())
        {
            decimal itemPrice = 0;

            foreach (var printingGroupService in printingGroupServices)
            {
                var catalogService = currentGroupServices.First(s => s.Id == printingGroupService.ServiceId);
                var servicePrice = GetservicePrice(catalogService);

                if (printingGroupService.IsCover)
                    itemPrice += servicePrice;
                else
                    itemPrice += CalculatePrintingServicePrice(item, servicePrice);
            }

            if (staplingService != null)
            {
                itemPrice += GetservicePrice(staplingService);
            }

            if (cluingService != null)
            {
                itemPrice += GetservicePrice(cluingService);
            }

            if (cuttingService != null)
            {
                itemPrice += GetservicePrice(cuttingService);
            }

            item.Price = itemPrice;

            // update unchanged status to modified to update database with new calcuated item price
            item.ObjectState = item.ObjectState == TrackingState.Unchanged ? TrackingState.Modified : item.ObjectState;

            decimal GetservicePrice(Service service)
            {
                return orderService.NotDeleted().First(x => service.Id == x.ServiceId).Price.GetValueOrDefault();
            }
        }
    }

    private void ValidateGroupServices(List<OrderGroupService> groupServices, List<Service> catalogServices)
    {
        var serviceById = catalogServices.ToDictionary(s => s.Id);

        var printingServices = groupServices
            .Where(gs => serviceById.TryGetValue(gs.ServiceId, out var s) && s.ServiceCategory?.Code == "Printing")
            .ToList();

        if (printingServices.Count(gs => !gs.IsCover) > 1)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.PrintingMainDuplicate));

        if (printingServices.Count(gs => gs.IsCover) > 1)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.PrintingCoverDuplicate));

        var nonPrintingCategoryCodes = groupServices
            .Select(gs => serviceById.TryGetValue(gs.ServiceId, out var s) ? s.ServiceCategory?.Code : null)
            .Where(code => !string.IsNullOrEmpty(code) && code != "Printing" && code != "Selling")
            .GroupBy(code => code)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (nonPrintingCategoryCodes.Count > 0)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.ServiceTypeDuplicate));
    }

    private decimal CalculatePrintingServicePrice(OrderItem item, decimal price)
    {
        string stringNoOfPages = item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages)?.Value;
        var noOfPages = string.IsNullOrEmpty(stringNoOfPages) ? 1 : int.Parse(stringNoOfPages);

        string stringNoOfPrintingFaces = item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces)?.Value;
        var noOfPrintingFaces = string.IsNullOrEmpty(stringNoOfPrintingFaces) ? 1 : int.Parse(stringNoOfPrintingFaces);

        return price * noOfPages / noOfPrintingFaces;
    }

    public async Task UpdateOrder(Guid id, OrderUpsertDto orderDTO, string userId)
    {
        var persisted = await LoadOrderGraphAsync(id, track: false);
        if (persisted is null)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        var executedItemIds = GetExecutedItemIds(persisted);
        ValidateOrderMutations(persisted, orderDTO, executedItemIds);

        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);
        PreservePersistedStatuses(order, persisted);

        ApplyZeroOrderFlag(order);
        ApplyZeroOrderPrices(order);
        order.TotalPrice = await CalculateOrderTotalPrice(order);

        _IUnitOfWork.OrderRepository.AddOrUpdate(order);

        await _IUnitOfWork.SaveChangesAsync(userId);
    }

    public async Task DeleteOrder(Guid id, string userId)
    {
        var persisted = await LoadOrderGraphAsync(id, track: true);

        if (persisted is null)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        if (persisted.Status == OrderStatusEnum.Delivered)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderAlreadyDelivered));

        var executedItemIds = GetExecutedItemIds(persisted);
        var hasCompletedOrDeliveredGroup = (persisted.OrderGroups ?? [])
            .Any(g => !g.IsDeleted && (g.Status == GroupStatusEnum.Completed || g.Status == GroupStatusEnum.Delivered));

        if (persisted.Status == OrderStatusEnum.Completed
            || persisted.Status == OrderStatusEnum.InProgress
            || hasCompletedOrDeliveredGroup
            || executedItemIds.Count > 0)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotDeleteOrderWithWork));

        _IUnitOfWork.OrderRepository.Remove(persisted);
        await _IUnitOfWork.SaveChangesAsync(userId);
    }

    private async Task<Order> LoadOrderGraphAsync(Guid orderId, bool track)
    {
        string[] includes = [
            $"{nameof(Order.OrderGroups)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}.{nameof(OrderItem.Details)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}"
        ];

        return await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync(order => order.Id == orderId, track, includes);
    }

    private List<Guid> GetActiveItemIds(IEnumerable<Guid> groupIds)
    {
        var ids = groupIds.ToList();
        if (ids.Count == 0)
            return [];

        return _IUnitOfWork.OrderItemRepository
            .Filter(i => ids.Contains(i.OrderGroupId) && !i.IsDeleted, track: false)
            .Select(i => i.Id)
            .ToList();
    }

    private HashSet<Guid> GetExecutedItemIds(IEnumerable<Guid> itemIds)
    {
        var ids = itemIds.ToList();
        if (ids.Count == 0)
            return [];

        return _IUnitOfWork.WorkerProductionRepository
            .Filter(e => ids.Contains(e.OrderItemId), track: false)
            .Select(e => e.OrderItemId)
            .ToHashSet();
    }

    private HashSet<Guid> GetExecutedItemIds(Order order)
    {
        var groupIds = (order.OrderGroups ?? []).Where(g => !g.IsDeleted).Select(g => g.Id);
        return GetExecutedItemIds(GetActiveItemIds(groupIds));
    }

    private void ApplyItemExecutionFlags(OrderDto orderDto, Order order)
    {
        var executedItemIds = GetExecutedItemIds(order);
        foreach (var group in orderDto.OrderGroups ?? [])
        {
            foreach (var item in group.Items ?? [])
                item.HasExecutions = executedItemIds.Contains(item.Id);
        }
    }

    private void ValidateOrderMutations(Order persisted, OrderUpsertDto incoming, HashSet<Guid> executedItemIds)
    {
        if (persisted.Status == OrderStatusEnum.Delivered)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderAlreadyDelivered));

        var persistedGroups = (persisted.OrderGroups ?? []).Where(g => !g.IsDeleted).ToDictionary(g => g.Id);
        var incomingGroups = incoming.OrderGroups ?? [];
        var incomingById = incomingGroups.ToDictionary(g => g.Id);

        foreach (var persistedGroup in persistedGroups.Values)
        {
            incomingById.TryGetValue(persistedGroup.Id, out var incomingGroup);
            var isGroupDeleted = incomingGroup is null || incomingGroup.ObjectState == TrackingState.Deleted;

            var groupItemIds = GetActiveItemIds([persistedGroup.Id]);
            var persistedItems = (persistedGroup.Items ?? []).Where(i => !i.IsDeleted).ToList();
            if (persistedItems.Count == 0 && groupItemIds.Count > 0)
            {
                persistedItems = _IUnitOfWork.OrderItemRepository
                    .Filter(i => i.OrderGroupId == persistedGroup.Id && !i.IsDeleted, track: false)
                    .ToList();
            }

            var incomingItems = incomingGroup?.Items ?? [];
            var groupHasExecutions = groupItemIds.Any(id => executedItemIds.Contains(id))
                || GetExecutedItemIds(groupItemIds).Count > 0;
            var groupIsClosed = persistedGroup.Status is GroupStatusEnum.Completed or GroupStatusEnum.Delivered;

            if (isGroupDeleted)
            {
                if (groupIsClosed)
                    ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotDeleteCompletedGroup));

                if (groupHasExecutions)
                    ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotDeleteGroupWithExecutions));

                if (groupItemIds.Count > 0)
                    ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotDeleteHasChildren));

                continue;
            }

            if (incomingGroup.ObjectState != TrackingState.Deleted
                && (groupHasExecutions || groupIsClosed)
                && GroupServicesOrTypeChanged(persistedGroup, incomingGroup))
            {
                ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotChangeServicesAfterExecution));
            }

            foreach (var incomingItem in incomingItems)
            {
                if (!persistedItems.Any(i => i.Id == incomingItem.Id))
                {
                    if (incomingItem.ObjectState == TrackingState.Added && groupIsClosed)
                        ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotAddItemToClosedGroup));
                    continue;
                }

                var persistedItem = persistedItems.First(i => i.Id == incomingItem.Id);
                var itemLocked = persistedItem.OrderItemStatus == OrderItemStatus.Completed
                    || executedItemIds.Contains(persistedItem.Id);

                if (!itemLocked)
                    continue;

                if (incomingItem.ObjectState == TrackingState.Deleted)
                    ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotDeleteExecutedItem));

                if (IsItemStructurallyChanged(persistedItem, incomingItem))
                    ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.CannotEditExecutedItem));
            }
        }
    }

    private static bool GroupServicesOrTypeChanged(OrderGroup persisted, OrderGroupUpsertDTO incoming)
    {
        if (persisted.ExecutionType != incoming.ExecutionType)
            return true;

        var persistedServiceKeys = (persisted.OrderGroupServices ?? [])
            .Where(s => !s.IsDeleted)
            .Select(s => (s.ServiceId, s.IsCover))
            .OrderBy(s => s.ServiceId)
            .ToList();

        var incomingServiceKeys = (incoming.OrderGroupServices ?? [])
            .Where(s => s.ObjectState != TrackingState.Deleted)
            .Select(s => (s.ServiceId, s.IsCover))
            .OrderBy(s => s.ServiceId)
            .ToList();

        return !persistedServiceKeys.SequenceEqual(incomingServiceKeys);
    }

    private static bool IsItemStructurallyChanged(OrderItem persisted, ItemUpsertDTO incoming)
    {
        if (incoming.ObjectState == TrackingState.Deleted)
            return false;

        if (!string.Equals(persisted.Name, incoming.Name, StringComparison.Ordinal)
            || persisted.Quantity != incoming.Quantity)
            return true;

        return DetailChanged(persisted, incoming, ItemDetailsKeyEnum.NumberOfPages)
            || DetailChanged(persisted, incoming, ItemDetailsKeyEnum.NumberOfPrintingFaces);
    }

    private static bool DetailChanged(OrderItem persisted, ItemUpsertDTO incoming, ItemDetailsKeyEnum key)
    {
        var persistedValue = persisted.Details?.FirstOrDefault(d => !d.IsDeleted && d.ItemDetailsKey == key)?.Value ?? "";
        var incomingValue = incoming.Details?.FirstOrDefault(d => d.Key == key && d.ObjectState != TrackingState.Deleted)?.Value ?? "";
        return !string.Equals(persistedValue, incomingValue, StringComparison.Ordinal);
    }

    private static void PreservePersistedStatuses(Order mapped, Order persisted)
    {
        mapped.Status = persisted.Status;
        mapped.TotalPaid = persisted.TotalPaid;

        var persistedGroups = (persisted.OrderGroups ?? []).ToDictionary(g => g.Id);
        foreach (var group in mapped.OrderGroups ?? [])
        {
            if (!persistedGroups.TryGetValue(group.Id, out var persistedGroup))
            {
                group.Status = GroupStatusEnum.New;
                continue;
            }

            group.Status = persistedGroup.Status;
            group.OrderId = persistedGroup.OrderId;
            group.DeliveryDate = persistedGroup.DeliveryDate;
            group.DeliveryName = persistedGroup.DeliveryName;
            group.ReceiverName = persistedGroup.ReceiverName;
            group.DeliveryNotes = persistedGroup.DeliveryNotes;

            var persistedItems = (persistedGroup.Items ?? []).ToDictionary(i => i.Id);
            foreach (var item in group.Items ?? [])
            {
                if (persistedItems.TryGetValue(item.Id, out var persistedItem))
                {
                    item.OrderItemStatus = persistedItem.OrderItemStatus;
                    item.OrderGroupId = persistedItem.OrderGroupId;
                }
                else
                {
                    item.OrderItemStatus = OrderItemStatus.New;
                }
            }
        }
    }
}
