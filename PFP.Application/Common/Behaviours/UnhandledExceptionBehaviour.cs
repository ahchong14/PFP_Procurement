using Microsoft.Extensions.Logging;
using PFP.Application.Abstractions.Messaging;
using PFP.Application.Common.Exceptions;

namespace PFP.Application.Common.Behaviours
{
    public sealed class UnhandledExceptionBehaviour<TRequest, TResponse>(
        ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex) when (ex is not (NotFoundException or ForbiddenException
                or UnauthorizedException or ValidationException
                or BusinessRuleException or AlreadySubmittedException))
            {
                logger.LogError(ex, "Unhandled exception for request {RequestName}", typeof(TRequest).Name);
                throw;
            }
        }
    }
}
