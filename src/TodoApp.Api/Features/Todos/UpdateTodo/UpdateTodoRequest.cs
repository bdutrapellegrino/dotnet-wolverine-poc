namespace TodoApp.Api.Features.Todos.UpdateTodo;

/// <summary>Normalized HTTP input contract for updating a todo (id comes from the route).</summary>
public record UpdateTodoRequest(string Title, string? Description);
