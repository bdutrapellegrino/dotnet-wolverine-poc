using FluentValidation;

namespace TodoApp.Application.Todos.CreateTodo;

/// <summary>
/// Command invariant, transport-agnostic. Enforced by Wolverine's message middleware on
/// EVERY dispatch through IMessageBus (HTTP, queue, cron, tests) — not just at the HTTP edge.
/// </summary>
public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
