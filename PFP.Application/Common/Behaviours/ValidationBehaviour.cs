using FluentValidation;
using PFP.Application.Abstractions.Messaging;
using ValidationException = PFP.Application.Common.Exceptions.ValidationException;

namespace PFP.Application.Common.Behaviours
{
    public sealed class ValidationBehaviour<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var failures = (await Task.WhenAll(
                        validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                    .SelectMany(r => r.Errors)
                    .Where(f => f is not null)
                    .ToList();

                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
