using Printpress.Domain;

namespace Printpress.Application;

internal sealed class GroupService(IUnitOfWork unitOfWork, ILocalizationService _loc) : IOrderGroupService
{
    public async Task<bool> DeliverGroup(DeliverGroupDto groupDeliveryDto, string userId)
    {

        var orderGroup = await unitOfWork.OrderGroupRepository.FirstOrDefaultAsync(x => x.Id == groupDeliveryDto.Id);

        if (orderGroup is null)
        {
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.GroupNotFound));
        }

        if (orderGroup.Status == GroupStatusEnum.Delivered || orderGroup.DeliveryDate.HasValue)
        {
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.GroupAlreadyDelivered, orderGroup.DeliveryDate?.ToString("yyyy-MM-dd")));
        }

        if (orderGroup.Status != GroupStatusEnum.Completed)
        {
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.GroupNotCompletedForDelivery));
        }

        orderGroup.DeliveryDate = groupDeliveryDto.DeliveryDate;
        orderGroup.DeliveryName = groupDeliveryDto.DeliveredFrom;
        orderGroup.ReceiverName = groupDeliveryDto.DeliveredTo;
        orderGroup.DeliveryNotes = groupDeliveryDto.DeliveryNotes;
        orderGroup.Status = GroupStatusEnum.Delivered;

        await unitOfWork.SaveChangesAsync(userId);

        bool allDelivered = IsAllOrderGroupDelivered(orderGroup.OrderId);
        if (allDelivered)
        {
            await MarkOrderAsDelivered(orderGroup.OrderId, userId);
        }

        return true;

    }
    private bool IsAllOrderGroupDelivered(Guid orderId)
    {
        var notDeliveredCount = unitOfWork.OrderGroupRepository.Count(x => x.OrderId == orderId && x.Status != GroupStatusEnum.Delivered && !x.IsDeleted);

        return notDeliveredCount == 0;

    }
    private async Task MarkOrderAsDelivered(Guid orderId, string userId)
    {
        var order = await unitOfWork.OrderRepository.FirstOrDefaultAsync(x => x.Id == orderId);

        if (order is null)
        {
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));
        }

        order.Status = OrderStatusEnum.Delivered;
        await unitOfWork.SaveChangesAsync(userId);
    }

}
