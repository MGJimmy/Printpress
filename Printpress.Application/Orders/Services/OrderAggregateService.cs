using Microsoft.EntityFrameworkCore;
using Printpress.Domain;
namespace Printpress.Application;

internal sealed class OrderAggregateService(IUnitOfWork _IUnitOfWork, OrderMapper _OrderMapper, IGuidGenerator _guidGenerator, ILocalizationService _loc) : IOrderAggregateService
{
    public async Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(
        int pageNumber,
        int pageSize,
        string search,
        Guid? clientId,
        OrderStatusEnum? status,
        bool? isZeroOrder,
        DateTime? dateFrom,
        DateTime? dateToExclusive)
    {
        var term = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        var orders = await _IUnitOfWork.OrderRepository.FilterAsync(
            new Paging(pageNumber, pageSize),
            o => (term == null || o.Name.Contains(term) || o.Client.Name.Contains(term))
                && (clientId == null || o.ClientId == clientId)
                && (status == null || o.Status == status)
                && (isZeroOrder == null || o.IsZeroOrder == isZeroOrder)
                && (dateFrom == null || o.CreatedAt >= dateFrom)
                && (dateToExclusive == null || o.CreatedAt < dateToExclusive),
            new Sorting(nameof(Order.CreatedAt), SortingDirection.DESC),
            nameof(Order.Client)
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

        foreach (var group in (order.OrderGroups ?? []).NotDeleted())
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
                var servicePrice = GetServicePrice(printingGroupService.ServiceId, printingGroupService.IsCover);

                if (printingGroupService.IsCover)
                    itemPrice += servicePrice;
                else
                    itemPrice += CalculatePrintingServicePrice(item, servicePrice);
            }

            if (staplingService != null)
            {
                itemPrice += GetServicePrice(staplingService.Id, false);
            }

            if (cluingService != null)
            {
                itemPrice += GetServicePrice(cluingService.Id, false);
            }

            if (cuttingService != null)
            {
                itemPrice += GetServicePrice(cuttingService.Id, false);
            }

            item.Price = itemPrice;

            // update unchanged status to modified to update database with new calcuated item price
            item.ObjectState = item.ObjectState == TrackingState.Unchanged ? TrackingState.Modified : item.ObjectState;

            decimal GetServicePrice(Guid serviceId, bool isCover)
            {
                return orderService.NotDeleted()
                    .First(x => x.ServiceId == serviceId && x.IsCover == isCover)
                    .Price.GetValueOrDefault();
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

    private HashSet<Guid> GetExecutedServiceCategoryIds(IEnumerable<Guid> itemIds)
    {
        var ids = itemIds.ToList();
        if (ids.Count == 0)
            return [];

        return _IUnitOfWork.WorkerProductionRepository
            .Filter(e => ids.Contains(e.OrderItemId), track: false)
            .Select(e => e.ServiceCategoryId)
            .ToHashSet();
    }

    private Dictionary<Guid, Guid> GetServiceCategoryMap(IEnumerable<Guid> serviceIds)
    {
        var ids = serviceIds.Distinct().ToList();
        if (ids.Count == 0)
            return [];

        return _IUnitOfWork.ServiceRepository
            .Filter(s => ids.Contains(s.Id), track: false)
            .ToDictionary(s => s.Id, s => s.ServiceCategoryId);
    }

    private HashSet<Guid> GetExecutedItemIds(Order order)
    {
        var groupIds = (order.OrderGroups ?? []).Where(g => !g.IsDeleted).Select(g => g.Id);
        return GetExecutedItemIds(GetActiveItemIds(groupIds));
    }

    private void ApplyItemExecutionFlags(OrderDto orderDto, Order order)
    {
        var itemIds = GetActiveItemIds((order.OrderGroups ?? []).Where(g => !g.IsDeleted).Select(g => g.Id));
        var productions = itemIds.Count == 0
            ? []
            : _IUnitOfWork.WorkerProductionRepository
                .Filter(e => itemIds.Contains(e.OrderItemId), track: false)
                .ToList();

        var executedItemIds = productions.Select(e => e.OrderItemId).ToHashSet();
        foreach (var group in orderDto.OrderGroups ?? [])
        {
            var groupItemIds = (group.Items ?? []).Select(i => i.Id).ToHashSet();
            group.ExecutedServiceCategoryIds = productions
                .Where(e => groupItemIds.Contains(e.OrderItemId))
                .Select(e => e.ServiceCategoryId)
                .Distinct()
                .ToList();

            foreach (var item in group.Items ?? [])
                item.HasExecutions = executedItemIds.Contains(item.Id);
        }
    }

    private void ValidateOrderMutations(Order persisted, OrderUpsertDto incoming, HashSet<Guid> executedItemIds)
    {
        if (persisted.Status == OrderStatusEnum.Delivered)
            Reject(LocalizationKeys.Orders.OrderAlreadyDelivered);

        var incomingById = (incoming.OrderGroups ?? []).ToDictionary(g => g.Id);

        foreach (var persistedGroup in (persisted.OrderGroups ?? []).Where(g => !g.IsDeleted))
        {
            incomingById.TryGetValue(persistedGroup.Id, out var incomingGroup);
            ValidateGroupMutation(persistedGroup, incomingGroup, executedItemIds);
        }
    }

    private void ValidateGroupMutation(
        OrderGroup persistedGroup,
        OrderGroupUpsertDTO incomingGroup,
        HashSet<Guid> executedItemIds)
    {
        var groupItemIds = GetActiveItemIds([persistedGroup.Id]);
        var groupIsClosed = IsGroupClosed(persistedGroup);
        var groupHasExecutions = GroupHasExecutions(groupItemIds, executedItemIds);

        if (incomingGroup is null || incomingGroup.ObjectState == TrackingState.Deleted)
        {
            EnsureGroupCanBeDeleted(groupIsClosed, groupHasExecutions, groupItemIds.Count);
            return;
        }

        EnsureGroupServicesCanChange(
            persistedGroup,
            incomingGroup,
            groupIsClosed,
            groupHasExecutions,
            groupItemIds);

        EnsureGroupItemsCanChange(
            LoadPersistedGroupItems(persistedGroup, groupItemIds),
            incomingGroup.Items ?? [],
            groupIsClosed,
            executedItemIds);
    }

    private static bool IsGroupClosed(OrderGroup group)
        => group.Status is GroupStatusEnum.Completed or GroupStatusEnum.Delivered;

    private bool GroupHasExecutions(List<Guid> groupItemIds, HashSet<Guid> executedItemIds)
        => groupItemIds.Any(executedItemIds.Contains)
            || GetExecutedItemIds(groupItemIds).Count > 0;

    private void EnsureGroupCanBeDeleted(bool groupIsClosed, bool groupHasExecutions, int itemCount)
    {
        if (groupIsClosed)
            Reject(LocalizationKeys.Orders.CannotDeleteCompletedGroup);

        if (groupHasExecutions)
            Reject(LocalizationKeys.Orders.CannotDeleteGroupWithExecutions);

        if (itemCount > 0)
            Reject(LocalizationKeys.Orders.CannotDeleteHasChildren);
    }

    private void EnsureGroupServicesCanChange(
        OrderGroup persistedGroup,
        OrderGroupUpsertDTO incomingGroup,
        bool groupIsClosed,
        bool groupHasExecutions,
        List<Guid> groupItemIds)
    {
        if (groupIsClosed && GroupServicesOrTypeChanged(persistedGroup, incomingGroup))
            Reject(LocalizationKeys.Orders.CannotChangeServicesAfterExecution);

        // After production: lock execution type, and block remove/swap of services already executed.
        // Adding unused services is still allowed.
        if (!groupIsClosed && groupHasExecutions
            && (persistedGroup.ExecutionType != incomingGroup.ExecutionType
                || HasRemovedExecutedGroupService(persistedGroup, incomingGroup, groupItemIds)))
            Reject(LocalizationKeys.Orders.CannotChangeServicesAfterExecution);
    }

    private List<OrderItem> LoadPersistedGroupItems(OrderGroup persistedGroup, List<Guid> groupItemIds)
    {
        var items = (persistedGroup.Items ?? []).Where(i => !i.IsDeleted).ToList();
        if (items.Count > 0 || groupItemIds.Count == 0)
            return items;

        return _IUnitOfWork.OrderItemRepository
            .Filter(i => i.OrderGroupId == persistedGroup.Id && !i.IsDeleted, track: false)
            .ToList();
    }

    private void EnsureGroupItemsCanChange(
        List<OrderItem> persistedItems,
        IEnumerable<ItemUpsertDTO> incomingItems,
        bool groupIsClosed,
        HashSet<Guid> executedItemIds)
    {
        foreach (var incomingItem in incomingItems)
            ValidateItemMutation(persistedItems, incomingItem, groupIsClosed, executedItemIds);
    }

    private void ValidateItemMutation(
        List<OrderItem> persistedItems,
        ItemUpsertDTO incomingItem,
        bool groupIsClosed,
        HashSet<Guid> executedItemIds)
    {
        var persistedItem = persistedItems.FirstOrDefault(i => i.Id == incomingItem.Id);
        if (persistedItem is null)
        {
            if (incomingItem.ObjectState == TrackingState.Added && groupIsClosed)
                Reject(LocalizationKeys.Orders.CannotAddItemToClosedGroup);
            return;
        }

        var itemLocked = persistedItem.OrderItemStatus == OrderItemStatus.Completed
            || executedItemIds.Contains(persistedItem.Id);

        if (!itemLocked)
            return;

        if (incomingItem.ObjectState == TrackingState.Deleted)
            Reject(LocalizationKeys.Orders.CannotDeleteExecutedItem);

        if (IsItemStructurallyChanged(persistedItem, incomingItem))
            Reject(LocalizationKeys.Orders.CannotEditExecutedItem);
    }

    private void Reject(string localizationKey)
        => ValidationExeption.FireValidationException(_loc.Get(localizationKey));

    private static bool GroupServicesOrTypeChanged(OrderGroup persisted, OrderGroupUpsertDTO incoming)
    {
        if (persisted.ExecutionType != incoming.ExecutionType)
            return true;

        var persistedServiceKeys = (persisted.OrderGroupServices ?? [])
            .Where(s => !s.IsDeleted)
            .Select(s => (s.ServiceId, s.IsCover))
            .ToHashSet();

        var incomingServiceKeys = (incoming.OrderGroupServices ?? [])
            .Where(s => s.ObjectState != TrackingState.Deleted)
            .Select(s => (s.ServiceId, s.IsCover))
            .ToHashSet();

        return !persistedServiceKeys.SetEquals(incomingServiceKeys);
    }

    // True when a persisted service was removed or swapped (ServiceId / IsCover)
    // and that service's category already has production on this group.
    private bool HasRemovedExecutedGroupService(
        OrderGroup persisted,
        OrderGroupUpsertDTO incoming,
        IEnumerable<Guid> groupItemIds)
    {
        var executedCategories = GetExecutedServiceCategoryIds(groupItemIds);
        if (executedCategories.Count == 0)
            return false;

        var incomingKeys = ActiveIncomingServiceKeys(incoming);

        var persistedServices = (persisted.OrderGroupServices ?? [])
            .Where(s => !s.IsDeleted)
            .ToList();

        var categoryByServiceId = GetServiceCategoryMap(persistedServices.Select(s => s.ServiceId));

        // Missing from incoming = deleted or replaced. Only those with production are blocked.
        return persistedServices.Any(service =>
            !incomingKeys.Contains((service.ServiceId, service.IsCover))
            && categoryByServiceId.TryGetValue(service.ServiceId, out var categoryId)
            && executedCategories.Contains(categoryId));
    }

    private static HashSet<(Guid ServiceId, bool IsCover)> ActiveIncomingServiceKeys(OrderGroupUpsertDTO incoming)
    {
        return (incoming.OrderGroupServices ?? [])
            .Where(s => s.ObjectState != TrackingState.Deleted)
            .Select(s => (s.ServiceId, s.IsCover))
            .ToHashSet();
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
