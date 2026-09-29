using Printpress.Domain;

namespace Printpress.Application;

internal sealed class GroupService(IUnitOfWork unitOfWork, ILocalizationService _loc) : IOrderGroupService
{
    public async Task<bool> DeliverGroup(DeliverGroupDto groupDeliveryDto, string userId)
    {
        var group = await unitOfWork.OrderGroupRepository.FirstOrDefaultAsync(x => x.Id == groupDeliveryDto.Id);

        if (group is null)
        {
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.GroupNotFound));
        }

        if (group.Status == GroupStatusEnum.Delivered || group.DeliveryDate.HasValue)
        {
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.GroupAlreadyDelivered, group.DeliveryDate?.ToString("yyyy-MM-dd")));
        }

        if (group.Status != GroupStatusEnum.Completed)
        {
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.GroupNotCompletedForDelivery));
        }

        var order = await unitOfWork.OrderRepository.FirstOrDefaultAsync(
            x => x.Id == group.OrderId,
            true,
            nameof(Order.OrderGroups));

        if (order is null)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        var trackedGroup = order.OrderGroups?.FirstOrDefault(g => g.Id == group.Id);
        if (trackedGroup is null)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.GroupNotFound));

        trackedGroup.DeliveryDate = groupDeliveryDto.DeliveryDate;
        trackedGroup.DeliveryName = groupDeliveryDto.DeliveredFrom;
        trackedGroup.ReceiverName = groupDeliveryDto.DeliveredTo;
        trackedGroup.DeliveryNotes = groupDeliveryDto.DeliveryNotes;
        trackedGroup.Status = GroupStatusEnum.Delivered;

        order.RefreshStatus();
        await unitOfWork.SaveChangesAsync(userId);

        return true;
    }
}
