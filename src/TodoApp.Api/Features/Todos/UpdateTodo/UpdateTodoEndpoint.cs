using Microsoft.AspNetCore.Http;
using TodoApp.Api.Features.Todos.Contracts;
using TodoApp.Application.Todos.Contracts;
using TodoApp.Application.Todos.UpdateTodo;
using Wolverine;
using Wolverine.Http;

namespace TodoApp.Api.Features.Todos.UpdateTodo;

/// <summary>
/// PUT /api/todos/{id} — validates <see cref="UpdateTodoRequest"/> at the boundary, then
/// combines it with the route id into the Application command. 404 when not found.
/// </summary>
public static class UpdateTodoEndpoint
{
    [WolverinePut("/api/todos/{id}")]
    public static async Task<IResult> Handle(
        Guid id,
        UpdateTodoRequest request,
        IMessageBus bus,
        CancellationToken ct)
    {
        var command = new UpdateTodoCommand(id, request.Title, request.Description);
        TodoDto? dto = await bus.InvokeAsync<TodoDto?>(command, ct);

        return dto is not null ? Results.Ok(dto.ToResponse()) : Results.NotFound();
    }
}
