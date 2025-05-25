using Printpress.Domain.Enums;
using Printpress.Enums;

namespace Printpress.Application;

internal sealed class GroupService(IUnitOfWork unitOfWork ) : IOrderGroupService
{
    public async Task<bool> DeliverGroup(DeliverGroupDto groupDeliveryDto)
    {

        var orderGroup = await unitOfWork.OrderGroupRepository.FirstOrDefaultAsync(x => x.Id == groupDeliveryDto.Id);

        if (orderGroup is null)
        {
            ValidationExeption.FireValidationException($"Order group with id {groupDeliveryDto.Id} not found.");
        }

        if (orderGroup.DeliveryDate.HasValue)
        {
            ValidationExeption.FireValidationException($"Order group with ID {groupDeliveryDto.Id} is already delivered on {orderGroup.DeliveryDate:yyyy-MM-dd}.");
        }

        orderGroup.DeliveryDate = groupDeliveryDto.DeliveryDate;
        orderGroup.DeliveryName = groupDeliveryDto.DeliveredFrom;
        orderGroup.ReceiverName = groupDeliveryDto.DeliveredTo;
        orderGroup.DeliveryNotes = groupDeliveryDto.DeliveryNotes;
        orderGroup.Status = GroupStatusEnum.Delivered;

        await unitOfWork.SaveChangesAsync();

        bool allDelivered = IsAllOrderGroupDelivered(orderGroup.OrderId);
        if (allDelivered)
        {
            await MarkOrderAsDelivered(orderGroup.OrderId);
        }

        return true;

    }
    private bool IsAllOrderGroupDelivered(int orderId)
    {
        var notDeliveredCount = unitOfWork.OrderGroupRepository.Count(x => x.OrderId == orderId && x.Status != GroupStatusEnum.Delivered && !x.IsDeleted);

        return notDeliveredCount == 0;

    }
    private async Task MarkOrderAsDelivered(int orderId)
    {
        var order = await unitOfWork.OrderRepository.FirstOrDefaultAsync(x => x.Id == orderId);

        if (order is null)
        {
            ValidationExeption.FireValidationException($"Order with ID {orderId} not found.");
        }

        order.Status = OrderStatusEnum.Delivered;
        await unitOfWork.SaveChangesAsync();
    }

}
