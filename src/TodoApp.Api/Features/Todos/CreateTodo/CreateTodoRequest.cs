namespace TodoApp.Api.Features.Todos.CreateTodo;

/// <summary>Normalized HTTP input contract for creating a todo.</summary>
public record CreateTodoRequest(string Title, string? Description);
