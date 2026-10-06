namespace TodoApp.Application.Todos.UpdateTodo;

public record UpdateTodoCommand(Guid Id, string Title, string? Description);
