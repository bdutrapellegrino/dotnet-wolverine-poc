using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Todos.Contracts;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Application.Todos.GetTodo;

public static class GetTodoHandler
{
    /// <summary>Returns null when the todo does not exist (mapped to 404 by the API).</summary>
    public static async Task<TodoDto?> Handle(GetTodoQuery query, TodoDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.Id, ct);

        return todo?.ToDto();
    }
}
