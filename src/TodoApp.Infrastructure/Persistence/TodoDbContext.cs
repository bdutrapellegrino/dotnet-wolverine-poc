using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Todos;

namespace TodoApp.Infrastructure.Persistence;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var todo = modelBuilder.Entity<TodoItem>();

        todo.ToTable("todos");
        todo.HasKey(x => x.Id);

        // EF Core materializes through the aggregate's private constructor and sets the
        // remaining state through its private setters — no public parameterless ctor needed.
        todo.Property(x => x.Title).IsRequired().HasMaxLength(200);
        todo.Property(x => x.Description).HasMaxLength(2000);
        todo.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        todo.Property(x => x.CreatedAt);
        todo.Property(x => x.CompletedAt);
    }
}
