using TodoApp.Application.Todos.Contracts;
using TodoApp.Domain.Todos;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Application.Todos.CreateTodo;

/// <summary>
/// Use-case handler. Uses the DbContext directly; Wolverine's transactional middleware
/// calls SaveChangesAsync and flushes the outbox after the handler returns.
/// </summary>
public static class CreateTodoHandler
{
    public static TodoDto Handle(CreateTodoCommand command, TodoDbContext db, TimeProvider clock)
    {
        var todo = TodoItem.Create(command.Title, command.Description, clock);
        db.Todos.Add(todo);
        return todo.ToDto();
    }
}
