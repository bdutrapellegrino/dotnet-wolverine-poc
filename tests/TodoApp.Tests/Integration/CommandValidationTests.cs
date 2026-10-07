using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TodoApp.Application.Todos.Contracts;
using TodoApp.Application.Todos.CreateTodo;
using Wolverine;
using Xunit;

namespace TodoApp.Tests.Integration;

[Collection(nameof(ApiCollection))]
public class CommandValidationTests(ApiFixture fixture)
{
    private readonly IServiceProvider _services = fixture.Host.Services;

    [Fact]
    public async Task invalid_command_is_rejected_on_the_bus_without_any_http()
    {
        using var scope = _services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        // Dispatch straight through the message bus (simulates a queue/cron/other handler).
        var act = async () => await bus.InvokeAsync<TodoDto>(new CreateTodoCommand("", null));

        await act.ShouldThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task valid_command_flows_through_the_bus()
    {
        using var scope = _services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        var dto = await bus.InvokeAsync<TodoDto>(new CreateTodoCommand("via bus", null));

        dto.Title.ShouldBe("via bus");
        dto.Status.ShouldBe("Pending");
    }
}
