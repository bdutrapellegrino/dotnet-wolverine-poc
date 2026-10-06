using TodoApp.Application.Todos.Contracts;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Application.Todos.UpdateTodo;

public static class UpdateTodoHandler
{
    /// <summary>Returns null when the todo does not exist (mapped to 404 by the API).</summary>
    public static async Task<TodoDto?> Handle(UpdateTodoCommand command, TodoDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos.FindAsync([command.Id], ct);
        if (todo is null)
        {
            return null;
        }

        todo.UpdateDetails(command.Title, command.Description);
        return todo.ToDto();
    }
}
