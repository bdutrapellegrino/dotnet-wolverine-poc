namespace TodoApp.Domain.Todos;

/// <summary>
/// Aggregate root for a single todo entry. Keeps its invariants internal so the
/// application layer only mutates it through intention-revealing methods.
/// </summary>
public class TodoItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TodoStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private TodoItem(Guid id, string title, string? description, DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = TodoStatus.Pending;
        CreatedAt = createdAt;
    }

    public static TodoItem Create(string title, string? description, TimeProvider? clock = null)
    {
        clock ??= TimeProvider.System;
        return new TodoItem(Guid.NewGuid(), title.Trim(), Normalize(description), clock.GetUtcNow());
    }

    public void UpdateDetails(string title, string? description)
    {
        Title = title.Trim();
        Description = Normalize(description);
    }

    public void MarkComplete(TimeProvider? clock = null)
    {
        if (Status == TodoStatus.Completed)
        {
            return;
        }

        clock ??= TimeProvider.System;
        Status = TodoStatus.Completed;
        CompletedAt = clock.GetUtcNow();
    }

    public void Reopen()
    {
        Status = TodoStatus.Pending;
        CompletedAt = null;
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
