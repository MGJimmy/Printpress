namespace Printpress.Application;

public interface IOrderGroupService
{
    Task<bool> DeliverGroup(DeliverGroupDto groupDeliveryDto, string userId);
}
