using FluentValidation;

namespace TodoApp.Api.Features.Todos.UpdateTodo;

public class UpdateTodoValidator : AbstractValidator<UpdateTodoRequest>
{
    public UpdateTodoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
