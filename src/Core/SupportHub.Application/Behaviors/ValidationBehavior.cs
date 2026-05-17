using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SupportHub.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validatorsList = validators.ToList();

        if (validatorsList.Count == 0)
            return await next(cancellationToken);
        
        logger.LogInformation("Validating request of type {RequestType}", typeof(TRequest).Name);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validatorsList.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            logger.LogWarning(
                "Validation failed for {RequestType} with {ErrorCount} errors",
                typeof(TRequest).Name,
                failures.Count);

            throw new ValidationException(failures);
        }

        logger.LogInformation(
            "Validation successful for {RequestType}",
            typeof(TRequest).Name);

        return await next(cancellationToken);
    }
}