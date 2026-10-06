namespace TodoApp.Application.Todos.ListTodos;

/// <summary>Status is a string here so the API layer does not depend on the domain enum.</summary>
public record ListTodosQuery(string? Status);
