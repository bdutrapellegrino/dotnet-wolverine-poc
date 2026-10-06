using TodoApp.Api.Features.Todos.Contracts;
using TodoApp.Application.Todos.Contracts;
using TodoApp.Application.Todos.ListTodos;
using Wolverine;
using Wolverine.Http;

namespace TodoApp.Api.Features.Todos.ListTodos;

/// <summary>GET /api/todos?status=Pending|Completed — dispatches a query, normalizes output.</summary>
public static class ListTodosEndpoint
{
    [WolverineGet("/api/todos")]
    public static async Task<IReadOnlyList<TodoResponse>> Handle(
        IMessageBus bus,
        string? status,
        CancellationToken ct)
    {
        IReadOnlyList<TodoDto> dtos = await bus.InvokeAsync<IReadOnlyList<TodoDto>>(
            new ListTodosQuery(status), ct);

        return dtos.Select(x => x.ToResponse()).ToList();
    }
}
