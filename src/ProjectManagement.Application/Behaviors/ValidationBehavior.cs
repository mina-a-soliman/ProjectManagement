using FluentValidation;
using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs FluentValidation validators before handler execution.
/// Returns a failure Result if validation fails, preventing the handler from executing.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = failures.Select(f => f.ErrorMessage).ToList();

        // Try to create a failure response if the response type is Result<T>
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod(nameof(Result<object>.FailureResponse),
                [typeof(string), typeof(List<string>)]);

            if (failureMethod is not null)
            {
                var result = failureMethod.Invoke(null, ["Validation failed.", errors]);
                return (TResponse)result!;
            }
        }

        throw new ValidationException(failures);
    }
}
