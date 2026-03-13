namespace Printpress.Application;

public interface IItemServiceExecutionService
{
    Task<OrderGroupItemsResponseDto> GetGroupItemsWithProgressAsync(Guid groupId);
    Task<ItemExecutionSummaryDto> GetItemExecutionSummaryAsync(Guid itemId);
    Task<ItemExecutionHistoryDto> GetItemExecutionHistoryAsync(Guid itemId);
    Task ExecuteAsync(ExecuteServiceRequestDto payload, string userId);
}
