namespace TodoApp.Application.Todos.CompleteTodo;

/// <summary>
/// Integration event published when a todo is completed. With the Wolverine EF Core outbox
/// it is persisted in the SAME transaction as the DbContext change and only dispatched after
/// the commit succeeds — no "saved but event lost" window.
/// </summary>
public record TodoCompleted(Guid Id);
