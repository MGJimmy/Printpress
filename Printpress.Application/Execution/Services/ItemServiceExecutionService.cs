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
            return BuildEmptyGroupResponse(group, distinctServiceCategories);

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
        if (payload.Workers == null || !payload.Workers.Any())
            throw new ValidationExeption("يجب إضافة عامل واحد على الأقل");

        if (payload.Workers.Any(w => w.Quantity <= 0))
            throw new ValidationExeption("كمية التنفيذ يجب أن تكون أكبر من صفر");

        var item = _unitOfWork.OrderItemRepository
            .FirstOrDefault(i => i.Id == payload.OrderItemId, nameof(OrderItem.OrderGroup));

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.OrderItemId));

        var group = item.OrderGroup ?? await _unitOfWork.OrderGroupRepository.FindAsync(item.OrderGroupId);
        await EnsureNotDeliveredAsync(group);

        if (item.OrderItemStatus == OrderItemStatus.Completed)
            throw new ValidationExeption("العنصر مكتمل بالفعل ولا يمكن تنفيذ خدمات عليه");

        var alreadyExecuted = _unitOfWork.WorkerProductionRepository
            .Filter(e => e.OrderItemId == payload.OrderItemId && e.ServiceCategoryId == payload.ServiceCategoryId)
            .Sum(e => e.Quantity);

        var newExecutionTotal = payload.Workers.Sum(w => w.Quantity);
        var remaining = RemainingQuantity(item.Quantity, alreadyExecuted);

        if (newExecutionTotal > remaining)
            throw new ValidationExeption(
                $"الكمية المطلوب تنفيذها ({newExecutionTotal}) تتجاوز الكمية المتبقية ({remaining})");

        var executionDate = UtcDateTime.AsUtc(payload.ExecutionDate);
        var executionRecords = payload.Workers.Select(w => CreateExecution(
            payload.OrderItemId,
            payload.ServiceCategoryId,
            w.WorkerId,
            w.Quantity,
            executionDate,
            payload.Notes)).ToList();

        await PersistExecutionsAsync(executionRecords, [item], userId);
    }

    public async Task ExecuteItemBatchAsync(ExecuteItemBatchRequestDto payload, string userId)
    {
        var serviceIds = GetBatchServiceIds(payload);

        var item = _unitOfWork.OrderItemRepository
            .FirstOrDefault(i => i.Id == payload.OrderItemId, nameof(OrderItem.OrderGroup));

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.OrderItemId));

        if (item.OrderItemStatus == OrderItemStatus.Completed)
            throw new ValidationExeption("العنصر مكتمل بالفعل ولا يمكن تنفيذ خدمات عليه");

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
        OrderGroup? group,
        IReadOnlyList<OrderItem> items,
        HashSet<Guid> serviceIds,
        ExecuteBatchRequestDto payload,
        string userId)
    {
        await EnsureNotDeliveredAsync(group);

        if (group is not null)
            EnsureServicesBelongToGroup(group.Id, serviceIds);

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
        await PersistExecutionsAsync(records, affectedItems, userId);
    }

    private async Task PersistExecutionsAsync(
        List<ItemServiceExecution> records,
        IReadOnlyList<OrderItem> affectedItems,
        string userId)
    {
        await _unitOfWork.WorkerProductionRepository.AddRange(records);

        var anyNewlyStarted = false;
        foreach (var item in affectedItems)
        {
            if (item.OrderItemStatus != OrderItemStatus.New)
                continue;

            item.OrderItemStatus = OrderItemStatus.InProgress;
            _unitOfWork.OrderItemRepository.Update(item);
            anyNewlyStarted = true;
        }

        await _unitOfWork.SaveChangesAsync(userId);

        if (anyNewlyStarted)
            await SetGroupAndOrderInProgressAsync(affectedItems[0].OrderGroupId, userId);

        await UpdateCompletionStatusAsync(affectedItems, userId);
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
                var remaining = RemainingQuantity(item.Quantity, AlreadyExecuted(executions, item.Id, serviceId));
                if (remaining <= 0)
                    continue;

                records.Add(CreateExecution(item.Id, serviceId, workerId, remaining, executionDate, notes));
            }
        }

        return records;
    }

    private ItemServiceExecution CreateExecution(
        Guid orderItemId,
        Guid serviceCategoryId,
        Guid workerId,
        int quantity,
        DateTime executionDate,
        string notes) => new()
    {
        Id = _guidGenerator.NewGuid(),
        OrderItemId = orderItemId,
        ServiceCategoryId = serviceCategoryId,
        WorkerId = workerId,
        Quantity = quantity,
        ExecutionDate = executionDate,
        Notes = notes
    };

    private async Task EnsureNotDeliveredAsync(OrderGroup? group)
    {
        if (group is null)
            return;

        if (group.Status == GroupStatusEnum.Delivered)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.CannotExecuteDelivered));

        var order = await _unitOfWork.OrderRepository.FindAsync(group.OrderId);
        if (order?.Status == OrderStatusEnum.Delivered)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.CannotExecuteDelivered));
    }

    private void EnsureServicesBelongToGroup(Guid groupId, HashSet<Guid> serviceIds)
    {
        var groupServiceIds = _unitOfWork.OrderGroupServiceRepository
            .Filter(gs => gs.OrderGroupId == groupId, nameof(OrderGroupService.Service))
            .Select(gs => gs.Service.ServiceCategoryId)
            .ToHashSet();

        if (!serviceIds.IsSubsetOf(groupServiceIds))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.ServiceNotFound));
    }

    private static int AlreadyExecuted(
        IEnumerable<ItemServiceExecution> executions,
        Guid orderItemId,
        Guid serviceCategoryId) =>
        executions
            .Where(e => e.OrderItemId == orderItemId && e.ServiceCategoryId == serviceCategoryId)
            .Sum(e => e.Quantity);

    private static int RemainingQuantity(int itemQuantity, int alreadyExecuted) =>
        itemQuantity - alreadyExecuted;


    private async Task SetGroupAndOrderInProgressAsync(Guid groupId, string userId)
    {
        var group = await _unitOfWork.OrderGroupRepository.FindAsync(groupId);
        if (group is null) return;

        bool groupUpdated = false;
        if (group.Status == GroupStatusEnum.New)
        {
            group.Status = GroupStatusEnum.InProgress;
            _unitOfWork.OrderGroupRepository.Update(group);
            groupUpdated = true;
        }

        if (groupUpdated)
            await _unitOfWork.SaveChangesAsync(userId);

        var order = await _unitOfWork.OrderRepository.FindAsync(group.OrderId);
        if (order is null) return;

        if (order.Status == OrderStatusEnum.New)
        {
            order.Status = OrderStatusEnum.InProgress;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(userId);
        }
    }

    private async Task UpdateCompletionStatusAsync(IReadOnlyList<OrderItem> items, string userId)
    {
        if (items.Count == 0)
            return;

        var anyComplete = false;
        foreach (var item in items)
        {
            if (await CheckAndUpdateItemStatusAsync(item, userId))
                anyComplete = true;
        }

        if (anyComplete)
            await CheckAndUpdateGroupStatusAsync(items[0].OrderGroupId, userId);
    }

    private async Task<bool> CheckAndUpdateItemStatusAsync(OrderItem item, string userId)
    {
        // Get all service categories for the item's group
        var groupServices = _unitOfWork.OrderGroupServiceRepository
            .Filter(gs => gs.OrderGroupId == item.OrderGroupId,
                nameof(OrderGroupService.Service),
                $"{nameof(OrderGroupService.Service)}.{nameof(Service.ServiceCategory)}")
            .ToList();

        if (!groupServices.Any()) return false;

        var distinctServiceCategoryIds = groupServices
            .Select(gs => gs.Service.ServiceCategoryId)
            .Distinct()
            .ToList();

        // Get all executions for this item
        var executions = _unitOfWork.WorkerProductionRepository
            .Filter(e => e.OrderItemId == item.Id)
            .ToList();

        // Check if all services are completed for this item
        bool allServicesComplete = distinctServiceCategoryIds.All(scId =>
            executions.Where(e => e.ServiceCategoryId == scId).Sum(e => e.Quantity) >= item.Quantity);

        if (!allServicesComplete) return false;

        var freshItem = await _unitOfWork.OrderItemRepository.FindAsync(item.Id);
        if (freshItem!.OrderItemStatus != OrderItemStatus.Completed)
        {
            freshItem.OrderItemStatus = OrderItemStatus.Completed;
            _unitOfWork.OrderItemRepository.Update(freshItem);
            await _unitOfWork.SaveChangesAsync(userId);
        }

        return true;
    }

    private async Task CheckAndUpdateGroupStatusAsync(Guid groupId, string userId)
    {
        var allGroupItems = _unitOfWork.OrderItemRepository
            .Filter(i => i.OrderGroupId == groupId)
            .ToList();

        if (!allGroupItems.Any()) return;

        bool allItemsComplete = allGroupItems.All(i => i.OrderItemStatus == OrderItemStatus.Completed);
        if (!allItemsComplete) return;

        var group = await _unitOfWork.OrderGroupRepository.FindAsync(groupId);
        if (group is null || group.Status is GroupStatusEnum.Completed or GroupStatusEnum.Delivered) return;

        group.Status = GroupStatusEnum.Completed;
        _unitOfWork.OrderGroupRepository.Update(group);
        await _unitOfWork.SaveChangesAsync(userId);

        // Check order
        await CheckAndUpdateOrderStatusAsync(group.OrderId, userId);
    }

    private async Task CheckAndUpdateOrderStatusAsync(Guid orderId, string userId)
    {
        var allGroups = _unitOfWork.OrderGroupRepository
            .Filter(g => g.OrderId == orderId)
            .ToList();

        if (!allGroups.Any()) return;

        bool allGroupsComplete = allGroups.All(g =>
            g.Status == GroupStatusEnum.Completed || g.Status == GroupStatusEnum.Delivered);

        if (!allGroupsComplete) return;

        var order = await _unitOfWork.OrderRepository.FindAsync(orderId);
        if (order is null || order.Status is OrderStatusEnum.Completed or OrderStatusEnum.Delivered) return;

        order.Status = OrderStatusEnum.Completed;
        _unitOfWork.OrderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(userId);
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
        OrderGroup group, List<ServiceCategory> serviceCategories) => new()
    {
        GroupId = group.Id,
        OrderId = group.OrderId,
        GroupName = group.Name,
        GroupStatus = group.Status.ToString(),
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
