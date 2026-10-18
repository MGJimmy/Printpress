namespace Printpress.Application;

public class TodoItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class TodoListDto
{
    public List<TodoItemDto> Open { get; set; }
    public List<TodoItemDto> Completed { get; set; }
}

public class TodoItemAddDto
{
    public string Title { get; set; }
}
