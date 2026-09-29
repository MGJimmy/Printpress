using Printpress.Domain;

namespace Printpress.Application;

internal sealed class ItemServiceExecutionService(
    IUnitOfWork _unitOfWork,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc) : IItemServiceExecutionService
{
    // ── Public Methods ───────────────────────────────────────────────────────

    public async Task<OrderGroupItemsResponseDto> GetGroupItemsWithProgressAsync(Guid groupId)
    {
        var group = await _unitOfWork.OrderGroupRepository.FindAsync(groupId);
        if (group is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(groupId));

        var order = await _unitOfWork.OrderRepository.FindAsync(group.OrderId);
        if (order is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        // Load group services → Service → ServiceCategory
        var groupServices = _unitOfWork.OrderGroupServiceRepository
            .Filter(gs => gs.OrderGroupId == groupId,
                nameof(OrderGroupService.Service),
                $"{nameof(OrderGroupService.Service)}.{nameof(Service.ServiceCategory)}")
            .ToList();

        var distinctServiceCategories = groupServices
            .Select(gs => gs.Service.ServiceCategory)
            .DistinctBy(sc => sc.Id)
            .ToList();

        // Load items for the group
        var items = _unitOfWork.OrderItemRepository
            .Filter(i => i.OrderGroupId == groupId)
            .ToList();

        if (!items.Any())
            return BuildEmptyGroupResponse(group, order.Status, distinctServiceCategories);

        var itemIds = items.Select(i => i.Id).ToList();

        // Load all executions for these items in one query
        var allExecutions = _unitOfWork.WorkerProductionRepository
            .Filter(e => itemIds.Contains(e.OrderItemId))
            .ToList();

        var itemDtos = items.Select(item =>
        {
            var itemExecutions = allExecutions.Where(e => e.OrderItemId == item.Id).ToList();
            return MapToItemWithProgress(item, distinctServiceCategories, itemExecutions);
        }).ToList();

        return new OrderGroupItemsResponseDto
        {
            GroupId = group.Id,
            OrderId = group.OrderId,
            GroupName = group.Name,
            GroupStatus = group.Status.ToString(),
            OrderStatus = order.Status.ToString(),
            GroupServices = distinctServiceCategories.Select(sc => new ServiceProgressDto
            {
                ServiceCategoryId = sc.Id,
                ServiceCategoryName = sc.Name,
                Executed = 0,
                Total = 0
            }).ToList(),
            Items = itemDtos
        };
    }

    public async Task<ItemExecutionSummaryDto> GetItemExecutionSummaryAsync(Guid itemId)
    {
        var item = _unitOfWork.OrderItemRepository
            .FirstOrDefault(i => i.Id == itemId, nameof(OrderItem.OrderGroup));

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(itemId));

        var groupServices = _unitOfWork.OrderGroupServiceRepository
            .Filter(gs => gs.OrderGroupId == item.OrderGroupId,
                nameof(OrderGroupService.Service),
                $"{nameof(OrderGroupService.Service)}.{nameof(Service.ServiceCategory)}")
            .ToList();

        var distinctServiceCategories = groupServices
            .Select(gs => gs.Service.ServiceCategory)
            .DistinctBy(sc => sc.Id)
            .ToList();

        var executions = _unitOfWork.WorkerProductionRepository
            .Filter(e => e.OrderItemId == itemId)
            .ToList();

        var serviceProgresses = distinctServiceCategories.Select(sc => BuildServiceProgress(
            sc, item.Quantity, executions.Where(e => e.ServiceCategoryId == sc.Id).Sum(e => e.Quantity)
        )).ToList();

        return new ItemExecutionSummaryDto
        {
            ItemId = item.Id,
            ItemName = item.Name,
            Quantity = item.Quantity,
            Status = item.OrderItemStatus.ToString(),
            GroupId = item.OrderGroupId,
            ServiceProgresses = serviceProgresses
        };
    }

    public async Task<ItemExecutionHistoryDto> GetItemExecutionHistoryAsync(Guid itemId)
    {
        var item = _unitOfWork.OrderItemRepository
            .FirstOrDefault(i => i.Id == itemId, nameof(OrderItem.OrderGroup));

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(itemId));

        var groupServices = _unitOfWork.OrderGroupServiceRepository
            .Filter(gs => gs.OrderGroupId == item.OrderGroupId,
                nameof(OrderGroupService.Service),
                $"{nameof(OrderGroupService.Service)}.{nameof(Service.ServiceCategory)}")
            .ToList();

        var distinctServiceCategories = groupServices
            .Select(gs => gs.Service.ServiceCategory)
            .DistinctBy(sc => sc.Id)
            .ToList();

        var executions = _unitOfWork.WorkerProductionRepository
            .Filter(e => e.OrderItemId == itemId,
                nameof(ItemServiceExecution.Worker),
                nameof(ItemServiceExecution.ServiceCategory))
            .OrderBy(e => e.ExecutionDate)
            .ToList();

        var serviceProgresses = distinctServiceCategories.Select(sc => BuildServiceProgress(
            sc, item.Quantity, executions.Where(e => e.ServiceCategoryId == sc.Id).Sum(e => e.Quantity)
        )).ToList();

        var records = executions.Select(e => new ItemExecutionRecordDto
        {
            Id = e.Id,
            WorkerName = e.Worker?.Name ?? string.Empty,
            ServiceCategoryName = e.ServiceCategory?.Name ?? string.Empty,
            Quantity = e.Quantity,
            ExecutionDate = e.ExecutionDate,
            Notes = e.Notes
        }).ToList();

        return new ItemExecutionHistoryDto
        {
            ItemId = item.Id,
            ItemName = item.Name,
            Quantity = item.Quantity,
            Status = item.OrderItemStatus.ToString(),
            GroupId = item.OrderGroupId,
            GroupName = item.OrderGroup?.Name ?? string.Empty,
            ServiceProgresses = serviceProgresses,
            ExecutionRecords = records
        };
    }

    public async Task ExecuteAsync(ExecuteServiceRequestDto payload, string userId)
    {
        if (payload?.Workers == null || !payload.Workers.Any())
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.WorkerRequired));

        if (payload.Workers.Any(w => w.WorkerId == Guid.Empty))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.WorkerRequired));

        if (payload.ServiceCategoryId == Guid.Empty)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.ServiceNotFound));

        if (payload.Workers.Any(w => w.Quantity <= 0))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.ExecutionQuantityMustBePositive));

        await EnsureWorkersExistAsync(payload.Workers.Select(w => w.WorkerId));

        var item = _unitOfWork.OrderItemRepository
            .FirstOrDefault(i => i.Id == payload.OrderItemId, nameof(OrderItem.OrderGroup));

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.OrderItemId));

        var (group, order, categoryIds) = await LoadExecutionContextAsync(item);

        group.EnsureCanExecute([payload.ServiceCategoryId], categoryIds);
        order.EnsureCanExecute();

        var alreadyExecuted = _unitOfWork.WorkerProductionRepository
            .Filter(e => e.OrderItemId == payload.OrderItemId && e.ServiceCategoryId == payload.ServiceCategoryId)
            .Sum(e => e.Quantity);

        item.AcceptExecution(payload.Workers.Sum(w => w.Quantity), alreadyExecuted);

        var executionDate = UtcDateTime.AsUtc(payload.ExecutionDate);
        var executionRecords = payload.Workers.Select(w => ItemServiceExecution.Create(
            _guidGenerator.NewGuid(),
            payload.OrderItemId,
            payload.ServiceCategoryId,
            w.WorkerId,
            w.Quantity,
            executionDate,
            payload.Notes)).ToList();

        await PersistExecutionsAsync(executionRecords, [item], group, order, categoryIds, userId);
    }

    public async Task ExecuteItemBatchAsync(ExecuteItemBatchRequestDto payload, string userId)
    {
        var serviceIds = GetBatchServiceIds(payload);

        var item = _unitOfWork.OrderItemRepository
            .FirstOrDefault(i => i.Id == payload.OrderItemId, nameof(OrderItem.OrderGroup));

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.OrderItemId));

        var group = item.OrderGroup ?? await _unitOfWork.OrderGroupRepository.FindAsync(item.OrderGroupId);
        await ExecuteRemainingBatchAsync(group, [item], serviceIds, payload, userId);
    }

    public async Task ExecuteGroupBatchAsync(ExecuteGroupBatchRequestDto payload, string userId)
    {
        var serviceIds = GetBatchServiceIds(payload);

        var group = await _unitOfWork.OrderGroupRepository.FindAsync(payload.GroupId);
        if (group is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.GroupId));

        var items = _unitOfWork.OrderItemRepository
            .Filter(i => i.OrderGroupId == payload.GroupId && i.OrderItemStatus != OrderItemStatus.Completed)
            .ToList();

        await ExecuteRemainingBatchAsync(group, items, serviceIds, payload, userId);
    }

    // ── Private Methods ──────────────────────────────────────────────────────

    private HashSet<Guid> GetBatchServiceIds(ExecuteBatchRequestDto payload)
    {
        if (payload.WorkerId == Guid.Empty)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.WorkerRequired));

        var serviceIds = (payload.ServiceCategoryIds ?? []).Where(id => id != Guid.Empty).ToHashSet();
        if (serviceIds.Count == 0)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.BatchEmptySelection));

        return serviceIds;
    }

    private async Task ExecuteRemainingBatchAsync(
        OrderGroup group,
        IReadOnlyList<OrderItem> items,
        HashSet<Guid> serviceIds,
        ExecuteBatchRequestDto payload,
        string userId)
    {
        if (group is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.GroupNotFound));

        var order = await _unitOfWork.OrderRepository.FindAsync(group.OrderId);
        if (order is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        var categoryIds = LoadGroupCategoryIds(group.Id);
        group.EnsureCanExecute(serviceIds, categoryIds);
        order.EnsureCanExecute();

        await EnsureWorkersExistAsync([payload.WorkerId]);

        if (!items.Any())
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.BatchNothingToExecute));

        var itemIds = items.Select(i => i.Id).ToList();
        var executions = _unitOfWork.WorkerProductionRepository
            .Filter(e => itemIds.Contains(e.OrderItemId))
            .ToList();

        var records = BuildRemainingRecords(
            items,
            serviceIds,
            executions,
            payload.WorkerId,
            UtcDateTime.AsUtc(payload.ExecutionDate),
            payload.Notes);

        if (records.Count == 0)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.BatchNothingToExecute));

        var affectedItems = items.Where(i => records.Any(r => r.OrderItemId == i.Id)).ToList();
        await PersistExecutionsAsync(records, affectedItems, group, order, categoryIds, userId);
    }

    private async Task PersistExecutionsAsync(
        List<ItemServiceExecution> records,
        IReadOnlyList<OrderItem> affectedItems,
        OrderGroup group,
        Order order,
        IReadOnlyCollection<Guid> requiredCategoryIds,
        string userId)
    {
        var itemIds = affectedItems.Select(i => i.Id).ToList();
        var existing = _unitOfWork.WorkerProductionRepository
            .Filter(e => itemIds.Contains(e.OrderItemId))
            .ToList();

        foreach (var item in affectedItems)
        {
            var executedByCategory = existing
                .Where(e => e.OrderItemId == item.Id)
                .Concat(records.Where(r => r.OrderItemId == item.Id))
                .GroupBy(e => e.ServiceCategoryId)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Quantity));

            item.TryComplete(requiredCategoryIds, executedByCategory);
            _unitOfWork.OrderItemRepository.Update(item);
        }

        await _unitOfWork.WorkerProductionRepository.AddRange(records);

        var allItems = _unitOfWork.OrderItemRepository
            .Filter(i => i.OrderGroupId == group.Id)
            .ToList();

        if (group.RefreshStatus(allItems.Select(i => i.OrderItemStatus)))
            _unitOfWork.OrderGroupRepository.Update(group);

        var allGroups = _unitOfWork.OrderGroupRepository
            .Filter(g => g.OrderId == order.Id)
            .ToList();

        order.OrderGroups = allGroups;
        if (order.RefreshStatus())
            _unitOfWork.OrderRepository.Update(order);

        await _unitOfWork.SaveChangesAsync(userId);
    }

    private List<ItemServiceExecution> BuildRemainingRecords(
        IReadOnlyList<OrderItem> items,
        HashSet<Guid> serviceIds,
        IReadOnlyList<ItemServiceExecution> executions,
        Guid workerId,
        DateTime executionDate,
        string notes)
    {
        var records = new List<ItemServiceExecution>();
        foreach (var item in items)
        {
            foreach (var serviceId in serviceIds)
            {
                var alreadyExecuted = executions
                    .Where(e => e.OrderItemId == item.Id && e.ServiceCategoryId == serviceId)
                    .Sum(e => e.Quantity);

                var remaining = item.GetRemaining(alreadyExecuted);
                if (remaining <= 0)
                    continue;

                item.AcceptExecution(remaining, alreadyExecuted);
                records.Add(ItemServiceExecution.Create(
                    _guidGenerator.NewGuid(),
                    item.Id,
                    serviceId,
                    workerId,
                    remaining,
                    executionDate,
                    notes));
            }
        }

        return records;
    }

    private async Task<(OrderGroup Group, Order Order, HashSet<Guid> CategoryIds)> LoadExecutionContextAsync(OrderItem item)
    {
        var group = item.OrderGroup ?? await _unitOfWork.OrderGroupRepository.FindAsync(item.OrderGroupId);
        if (group is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.GroupNotFound));

        var order = await _unitOfWork.OrderRepository.FindAsync(group.OrderId);
        if (order is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        return (group, order, LoadGroupCategoryIds(group.Id));
    }

    private HashSet<Guid> LoadGroupCategoryIds(Guid groupId) =>
        _unitOfWork.OrderGroupServiceRepository
            .Filter(gs => gs.OrderGroupId == groupId, nameof(OrderGroupService.Service))
            .Select(gs => gs.Service.ServiceCategoryId)
            .ToHashSet();

    private async Task EnsureWorkersExistAsync(IEnumerable<Guid> workerIds)
    {
        var ids = workerIds.Distinct().ToList();
        if (ids.Count == 0 || !await _unitOfWork.WorkerRepository.AllExistAsync(ids))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.WorkerNotFound));
    }

    private static ItemWithServiceProgressDto MapToItemWithProgress(
        OrderItem item,
        List<ServiceCategory> serviceCategories,
        List<ItemServiceExecution> executions)
    {
        var serviceProgresses = serviceCategories.Select(sc =>
        {
            var executed = executions
                .Where(e => e.ServiceCategoryId == sc.Id)
                .Sum(e => e.Quantity);
            return BuildServiceProgress(sc, item.Quantity, executed);
        }).ToList();

        return new ItemWithServiceProgressDto
        {
            Id = item.Id,
            Name = item.Name,
            Quantity = item.Quantity,
            Status = item.OrderItemStatus.ToString(),
            ServiceProgresses = serviceProgresses
        };
    }

    private static ServiceProgressDto BuildServiceProgress(ServiceCategory sc, int total, int executed) => new()
    {
        ServiceCategoryId = sc.Id,
        ServiceCategoryName = sc.Name,
        Executed = executed,
        Total = total
    };

    private static OrderGroupItemsResponseDto BuildEmptyGroupResponse(
        OrderGroup group, OrderStatusEnum orderStatus, List<ServiceCategory> serviceCategories) => new()
    {
        GroupId = group.Id,
        OrderId = group.OrderId,
        GroupName = group.Name,
        GroupStatus = group.Status.ToString(),
        OrderStatus = orderStatus.ToString(),
        GroupServices = serviceCategories.Select(sc => new ServiceProgressDto
        {
            ServiceCategoryId = sc.Id,
            ServiceCategoryName = sc.Name,
            Executed = 0,
            Total = 0
        }).ToList(),
        Items = []
    };
}
