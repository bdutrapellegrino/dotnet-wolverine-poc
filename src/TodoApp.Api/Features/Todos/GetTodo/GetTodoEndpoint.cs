using Microsoft.AspNetCore.Http;
using TodoApp.Api.Features.Todos.Contracts;
using TodoApp.Application.Todos.Contracts;
using TodoApp.Application.Todos.GetTodo;
using Wolverine;
using Wolverine.Http;

namespace TodoApp.Api.Features.Todos.GetTodo;

/// <summary>GET /api/todos/{id} — dispatches a query; 404 when the handler returns null.</summary>
public static class GetTodoEndpoint
{
    [WolverineGet("/api/todos/{id}")]
    public static async Task<IResult> Handle(Guid id, IMessageBus bus, CancellationToken ct)
    {
        TodoDto? dto = await bus.InvokeAsync<TodoDto?>(new GetTodoQuery(id), ct);
        return dto is not null ? Results.Ok(dto.ToResponse()) : Results.NotFound();
    }
}
