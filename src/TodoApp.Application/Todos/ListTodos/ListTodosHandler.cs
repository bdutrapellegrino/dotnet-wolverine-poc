using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Todos.Contracts;
using TodoApp.Domain.Todos;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Application.Todos.ListTodos;

public static class ListTodosHandler
{
    public static async Task<IReadOnlyList<TodoDto>> Handle(ListTodosQuery query, TodoDbContext db, CancellationToken ct)
    {
        TodoStatus? status = Enum.TryParse<TodoStatus>(query.Status, ignoreCase: true, out var parsed)
            ? parsed
            : null;

        var q = db.Todos.AsNoTracking();
        if (status is not null)
        {
            q = q.Where(x => x.Status == status);
        }

        // Postgres orders by timestamptz server-side (unlike SQLite).
        var items = await q.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
        return items.Select(x => x.ToDto()).ToList();
    }
}
