namespace Printpress.Domain;

public class TodoItem : Entity
{
    public const int TitleMaxLength = 200;

    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private TodoItem()
    {
    }

    public TodoItem(string title)
    {
        ApplyTitle(title);
        IsCompleted = false;
        CompletedAt = null;
    }

    public void Complete()
    {
        if (IsCompleted)
            throw new BusinessExceptions(LocalizationKeys.Todos.AlreadyCompleted);

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
    }

    private void ApplyTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessExceptions(LocalizationKeys.Todos.TitleRequired);

        var trimmed = title.Trim();
        if (trimmed.Length > TitleMaxLength)
            throw new BusinessExceptions(LocalizationKeys.Todos.TitleMaxLength);

        Title = trimmed;
    }
}
