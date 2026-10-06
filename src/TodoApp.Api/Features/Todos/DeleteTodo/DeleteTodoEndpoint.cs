using Microsoft.AspNetCore.Http;
using TodoApp.Application.Todos.DeleteTodo;
using Wolverine;
using Wolverine.Http;

namespace TodoApp.Api.Features.Todos.DeleteTodo;

/// <summary>DELETE /api/todos/{id} — 204 on success, 404 when the handler reports not found.</summary>
public static class DeleteTodoEndpoint
{
    [WolverineDelete("/api/todos/{id}")]
    public static async Task<IResult> Handle(Guid id, IMessageBus bus, CancellationToken ct)
    {
        var deleted = await bus.InvokeAsync<bool>(new DeleteTodoCommand(id), ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
