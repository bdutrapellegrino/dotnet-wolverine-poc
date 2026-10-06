using TodoApp.Application.Todos.Contracts;
using TodoApp.Infrastructure.Persistence;
using Wolverine;

namespace TodoApp.Application.Todos.CompleteTodo;

public static class CompleteTodoHandler
{
    /// <summary>
    /// Returns null when the todo does not exist (mapped to 404 by the API). Idempotent.
    /// Publishes <see cref="TodoCompleted"/> through the Wolverine outbox so it commits
    /// atomically with the status change.
    /// </summary>
    public static async Task<TodoDto?> Handle(
        CompleteTodoCommand command,
        TodoDbContext db,
        IMessageContext messaging,
        TimeProvider clock,
        CancellationToken ct)
    {
        var todo = await db.Todos.FindAsync([command.Id], ct);
        if (todo is null)
        {
            return null;
        }

        todo.MarkComplete(clock);
        await messaging.PublishAsync(new TodoCompleted(todo.Id));

        return todo.ToDto();
    }
}
