namespace Printpress.Application;

public interface ITodoItemService
{
    Task<TodoListDto> GetListAsync();
    Task<TodoItemDto> AddAsync(TodoItemAddDto payload, string userId);
    Task CompleteAsync(Guid id, string userId);
}
