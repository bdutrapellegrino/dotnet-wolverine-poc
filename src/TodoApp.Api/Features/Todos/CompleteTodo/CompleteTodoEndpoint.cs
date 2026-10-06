using Microsoft.AspNetCore.Http;
using TodoApp.Api.Features.Todos.Contracts;
using TodoApp.Application.Todos.CompleteTodo;
using TodoApp.Application.Todos.Contracts;
using Wolverine;
using Wolverine.Http;

namespace TodoApp.Api.Features.Todos.CompleteTodo;

/// <summary>POST /api/todos/{id}/complete — body-less, so no concrete body parameter here.</summary>
public static class CompleteTodoEndpoint
{
    [WolverinePost("/api/todos/{id}/complete")]
    public static async Task<IResult> Handle(Guid id, IMessageBus bus, CancellationToken ct)
    {
        TodoDto? dto = await bus.InvokeAsync<TodoDto?>(new CompleteTodoCommand(id), ct);
        return dto is not null ? Results.Ok(dto.ToResponse()) : Results.NotFound();
    }
}
