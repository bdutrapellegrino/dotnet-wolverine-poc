using TodoApp.Domain.Todos;

namespace TodoApp.Application.Todos.Contracts;

/// <summary>
/// Use-case result returned by the Application layer. It is the application's own output
/// model (not the HTTP contract) — the API maps this to its normalized Response DTO so the
/// transport shape can evolve independently from the use-case shape.
/// </summary>
public record TodoDto(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);

internal static class TodoMappings
{
    public static TodoDto ToDto(this TodoItem item) => new(
        item.Id,
        item.Title,
        item.Description,
        item.Status.ToString(),
        item.CreatedAt,
        item.CompletedAt);
}
