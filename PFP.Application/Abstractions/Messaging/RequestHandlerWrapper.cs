using Microsoft.Extensions.DependencyInjection;

namespace PFP.Application.Abstractions.Messaging;

internal sealed class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerBase
    where TRequest : IRequest<TResponse>
{
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        Task<TResponse> RootHandler() =>
            serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                .Handle((TRequest)request, cancellationToken);

        var pipeline = serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate(
                (RequestHandlerDelegate<TResponse>)RootHandler,
                (next, behaviour) => () => behaviour.Handle((TRequest)request, next, cancellationToken));

        return await pipeline();
    }
}
