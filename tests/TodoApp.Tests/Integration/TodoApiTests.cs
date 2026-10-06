using Alba;
using Shouldly;
using TodoApp.Api.Features.Todos.Contracts;
using TodoApp.Api.Features.Todos.CreateTodo;
using TodoApp.Api.Features.Todos.UpdateTodo;
using Wolverine.Http;
using Xunit;

namespace TodoApp.Tests.Integration;

[Collection(nameof(ApiCollection))]
public class TodoApiTests(ApiFixture fixture)
{
    private readonly IAlbaHost _host = fixture.Host;

    [Fact]
    public async Task post_with_blank_title_is_rejected_at_the_boundary()
    {
        await _host.Scenario(x =>
        {
            x.Post.Json(new CreateTodoRequest("", null)).ToUrl("/api/todos");
            x.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task full_lifecycle_create_get_complete_list()
    {
        // create
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Json(new CreateTodoRequest("Buy coffee", "grounds")).ToUrl("/api/todos");
            x.StatusCodeShouldBe(201);
        });
        var created = createResult.ReadAsJson<CreationResponse<TodoResponse>>()!.Value;
        created.Title.ShouldBe("Buy coffee");
        created.Status.ShouldBe("Pending");

        // get
        var getResult = await _host.Scenario(x =>
        {
            x.Get.Url($"/api/todos/{created.Id}");
            x.StatusCodeShouldBe(200);
        });
        getResult.ReadAsJson<TodoResponse>().Id.ShouldBe(created.Id);

        // update
        await _host.Scenario(x =>
        {
            x.Put.Json(new UpdateTodoRequest("Buy espresso", null)).ToUrl($"/api/todos/{created.Id}");
            x.StatusCodeShouldBe(200);
        });

        // complete (publishes TodoCompleted through the outbox)
        var completeResult = await _host.Scenario(x =>
        {
            x.Post.Url($"/api/todos/{created.Id}/complete");
            x.StatusCodeShouldBe(200);
        });
        completeResult.ReadAsJson<TodoResponse>().Status.ShouldBe("Completed");

        // list filtered by status
        var listResult = await _host.Scenario(x =>
        {
            x.Get.Url("/api/todos?status=Completed");
            x.StatusCodeShouldBe(200);
        });
        listResult.ReadAsJson<TodoResponse[]>()
            .ShouldContain(t => t.Id == created.Id);
    }

    [Fact]
    public async Task get_missing_todo_returns_404()
    {
        await _host.Scenario(x =>
        {
            x.Get.Url($"/api/todos/{Guid.NewGuid()}");
            x.StatusCodeShouldBe(404);
        });
    }
}
