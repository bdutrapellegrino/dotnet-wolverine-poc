using TodoApp.Application.Todos.Contracts;

namespace TodoApp.Api.Features.Todos.Contracts;

/// <summary>
/// Normalized HTTP output contract owned by the API. The API maps the Application's
/// <see cref="TodoDto"/> into this, so the transport shape is decoupled from the use-case shape.
/// </summary>
public record TodoResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);

public static class TodoResponseMappings
{
    public static TodoResponse ToResponse(this TodoDto dto) => new(
        dto.Id,
        dto.Title,
        dto.Description,
        dto.Status,
        dto.CreatedAt,
        dto.CompletedAt);
}
