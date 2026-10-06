using FluentValidation;

namespace TodoApp.Api.Features.Todos.CreateTodo;

/// <summary>Validates the incoming request at the API boundary (400 ProblemDetails).</summary>
public class CreateTodoValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
