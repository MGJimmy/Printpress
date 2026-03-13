using Printpress.Domain;

namespace Printpress.Application;

internal sealed class ItemServiceExecutionService(
    IUnitOfWork _unitOfWork,
    IGuidGenerator _guidGenerator) : IItemServiceExecutionService
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

        if (item.OrderItemStatus == OrderItemStatus.Completed)
            throw new ValidationExeption("العنصر مكتمل بالفعل ولا يمكن تنفيذ خدمات عليه");

        // Validate total new execution doesn't exceed item quantity
        var alreadyExecuted = _unitOfWork.WorkerProductionRepository
            .Filter(e => e.OrderItemId == payload.OrderItemId && e.ServiceCategoryId == payload.ServiceCategoryId)
            .Sum(e => e.Quantity);

        var newExecutionTotal = payload.Workers.Sum(w => w.Quantity);

        if (alreadyExecuted + newExecutionTotal > item.Quantity)
            throw new ValidationExeption(
                $"الكمية المطلوب تنفيذها ({newExecutionTotal}) تتجاوز الكمية المتبقية ({item.Quantity - alreadyExecuted})");

        // Create execution records per worker
        var executionRecords = payload.Workers.Select(w => new ItemServiceExecution
        {
            Id = _guidGenerator.NewGuid(),
            OrderItemId = payload.OrderItemId,
            ServiceCategoryId = payload.ServiceCategoryId,
            WorkerId = w.WorkerId,
            Quantity = w.Quantity,
            ExecutionDate = DateTime.SpecifyKind(payload.ExecutionDate, DateTimeKind.Utc),
            Notes = payload.Notes
        }).ToList();

        await _unitOfWork.WorkerProductionRepository.AddRange(executionRecords);

        // Update item/group/order status to InProgress if first execution
        bool wasNew = item.OrderItemStatus == OrderItemStatus.New;
        if (wasNew)
        {
            item.OrderItemStatus = OrderItemStatus.InProgress;
            _unitOfWork.OrderItemRepository.Update(item);
        }

        await _unitOfWork.SaveChangesAsync(userId);

        // Set group and order to InProgress if they were New
        if (wasNew)
            await SetGroupAndOrderInProgressAsync(item.OrderGroupId, userId);

        // Check completion chain (re-read for fresh data)
        await UpdateCompletionStatusAsync(item, userId);
    }

    // ── Private Methods ──────────────────────────────────────────────────────

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

    private async Task UpdateCompletionStatusAsync(OrderItem item, string userId)
    {
        bool itemIsNowComplete = await CheckAndUpdateItemStatusAsync(item, userId);

        if (itemIsNowComplete)
        {
            await CheckAndUpdateGroupStatusAsync(item.OrderGroupId, userId);
        }
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
        if (group is null || group.Status == GroupStatusEnum.Completed) return;

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
        if (order is null || order.Status == OrderStatusEnum.Completed) return;

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
