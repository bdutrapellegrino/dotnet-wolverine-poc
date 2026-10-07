using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace TodoApp.Api;

/// <summary>
/// Translates a FluentValidation <see cref="ValidationException"/> — thrown by Wolverine's
/// message-bus validation middleware when a command is invalid — into a 400 ProblemDetails
/// (RFC 9110, application/problem+json) with a per-field <c>errors</c> dictionary.
/// This makes command-level validation the single source of truth while keeping clean
/// HTTP 400s, without duplicating rules at the HTTP boundary.
/// </summary>
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validation)
        {
            return false;
        }

        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        await Results.ValidationProblem(errors).ExecuteAsync(httpContext);
        return true;
    }
}
