using Microsoft.Extensions.Logging;

namespace TodoApp.Application.Todos.CompleteTodo;

/// <summary>
/// Reacts to <see cref="TodoCompleted"/>. Dispatched by Wolverine from the durable outbox
/// after the completing transaction commits. Here it just logs — in a real system this is
/// where you'd send a notification, update a projection, call another service, etc.
/// </summary>
public static class TodoCompletedHandler
{
    public static void Handle(TodoCompleted @event, ILogger<TodoCompleted> logger)
        => logger.LogInformation("TodoCompleted handled for {TodoId} (via Wolverine outbox).", @event.Id);
}
