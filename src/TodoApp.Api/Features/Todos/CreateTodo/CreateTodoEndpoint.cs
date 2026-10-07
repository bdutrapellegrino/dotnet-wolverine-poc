using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Features.Todos.Contracts;
using TodoApp.Application.Todos.Contracts;
using TodoApp.Application.Todos.CreateTodo;
using Wolverine;
using Wolverine.Http;

namespace TodoApp.Api.Features.Todos.CreateTodo;

/// <summary>
/// POST /api/todos — FastEndpoints-style slice: the request body is the API's own
/// <see cref="CreateTodoRequest"/>, validated at the HTTP boundary by
/// <see cref="CreateTodoValidator"/> (400 ProblemDetails). The endpoint then maps the
/// request to the Application command, dispatches it, and normalizes the result into
/// the API's <see cref="TodoResponse"/>.
/// </summary>
public static class CreateTodoEndpoint
{
    [WolverinePost("/api/todos")]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public static async Task<CreationResponse<TodoResponse>> Handle(
        CreateTodoRequest request,
        IMessageBus bus,
        CancellationToken ct)
    {
        var command = new CreateTodoCommand(request.Title, request.Description);
        TodoDto dto = await bus.InvokeAsync<TodoDto>(command, ct);

        var response = dto.ToResponse();
        return new CreationResponse<TodoResponse>($"/api/todos/{response.Id}", response);
    }
}
