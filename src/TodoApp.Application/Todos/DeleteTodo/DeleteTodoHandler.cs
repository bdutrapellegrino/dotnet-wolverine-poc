using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Application.Todos.DeleteTodo;

public static class DeleteTodoHandler
{
    /// <summary>Returns false when the todo does not exist (mapped to 404 by the API).</summary>
    public static async Task<bool> Handle(DeleteTodoCommand command, TodoDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos.FindAsync([command.Id], ct);
        if (todo is null)
        {
            return false;
        }

        db.Todos.Remove(todo);
        return true;
    }
}
