using Microsoft.Extensions.Time.Testing;
using Shouldly;
using TodoApp.Domain.Todos;
using Xunit;

namespace TodoApp.Tests.Domain;

public class TodoItemTests
{
    [Fact]
    public void Create_starts_pending_and_trims_title()
    {
        var todo = TodoItem.Create("  Buy coffee  ", "  grounds  ");

        todo.Id.ShouldNotBe(Guid.Empty);
        todo.Title.ShouldBe("Buy coffee");
        todo.Description.ShouldBe("grounds");
        todo.Status.ShouldBe(TodoStatus.Pending);
        todo.CompletedAt.ShouldBeNull();
    }

    [Fact]
    public void Create_normalizes_blank_description_to_null()
    {
        var todo = TodoItem.Create("task", "   ");
        todo.Description.ShouldBeNull();
    }

    [Fact]
    public void MarkComplete_sets_status_and_timestamp_from_clock()
    {
        var clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
        var todo = TodoItem.Create("task", null, clock);

        todo.MarkComplete(clock);

        todo.Status.ShouldBe(TodoStatus.Completed);
        todo.CompletedAt.ShouldBe(clock.GetUtcNow());
    }

    [Fact]
    public void MarkComplete_is_idempotent()
    {
        var clock = new FakeTimeProvider();
        var todo = TodoItem.Create("task", null, clock);

        todo.MarkComplete(clock);
        var firstCompletedAt = todo.CompletedAt;
        clock.Advance(TimeSpan.FromHours(1));
        todo.MarkComplete(clock);

        todo.CompletedAt.ShouldBe(firstCompletedAt);
    }

    [Fact]
    public void Reopen_clears_completion()
    {
        var todo = TodoItem.Create("task", null);
        todo.MarkComplete();

        todo.Reopen();

        todo.Status.ShouldBe(TodoStatus.Pending);
        todo.CompletedAt.ShouldBeNull();
    }
}
