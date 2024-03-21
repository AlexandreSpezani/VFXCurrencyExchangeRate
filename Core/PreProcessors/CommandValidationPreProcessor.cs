using FluentValidation;
using MediatR.Pipeline;

namespace Core.PreProcessors;

public class CommandValidationPreProcessor<TRequest>(IValidator<TRequest>? validator = null)
    : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    // To validate mediatr commands before they are handled
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
        }
    }
}