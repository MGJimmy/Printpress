using Printpress.Domain;

namespace Printpress.Application;

internal sealed class TodoItemService(
    IUnitOfWork unitOfWork,
    IGuidGenerator guidGenerator,
    ILocalizationService _loc) : ITodoItemService
{
    public async Task<TodoListDto> GetListAsync()
    {
        var items = await unitOfWork.TodoItemRepository.AllAsync();

        return new TodoListDto
        {
            Open = items
                .Where(item => !item.IsCompleted)
                .OrderByDescending(item => item.CreatedAt)
                .Select(Map)
                .ToList(),
            Completed = items
                .Where(item => item.IsCompleted)
                .OrderByDescending(item => item.CompletedAt)
                .Select(Map)
                .ToList()
        };
    }

    public async Task<TodoItemDto> AddAsync(TodoItemAddDto payload, string userId)
    {
        if (payload is null || string.IsNullOrWhiteSpace(payload.Title))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Todos.TitleRequired));

        var item = new TodoItem(payload.Title);
        item.Id = guidGenerator.NewGuid();
        await unitOfWork.TodoItemRepository.AddAsync(item);
        await unitOfWork.SaveChangesAsync(userId);
        return Map(item);
    }

    public async Task CompleteAsync(Guid id, string userId)
    {
        var item = await unitOfWork.TodoItemRepository.FindAsync(id)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Todos.NotFound));

        item.Complete();
        unitOfWork.TodoItemRepository.Update(item);
        await unitOfWork.SaveChangesAsync(userId);
    }

    private static TodoItemDto Map(TodoItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        IsCompleted = item.IsCompleted,
        CompletedAt = item.CompletedAt
    };
}
