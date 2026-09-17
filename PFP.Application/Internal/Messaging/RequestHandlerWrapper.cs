using Microsoft.Extensions.DependencyInjection;
using PFP.Application.Abstractions.Messaging;

namespace PFP.Application.Internal.Messaging;

internal sealed class RequestHandlerWrapper<TRequest, TResponse>
    : RequestHandlerBase
    where TRequest : IRequest<TResponse>
{
    public override async Task<object?> Handle(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Task<TResponse> RootHandler()
        {
            IRequestHandler<TRequest, TResponse> handler =
                serviceProvider
                    .GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            return handler.Handle(
                (TRequest)request,
                cancellationToken);
        }

        List<IPipelineBehavior<TRequest, TResponse>> behaviours =
            serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .ToList();

        behaviours.Reverse();

        RequestHandlerDelegate<TResponse> pipeline = RootHandler;

        foreach (IPipelineBehavior<TRequest, TResponse>? behaviour in behaviours)
        {
            RequestHandlerDelegate<TResponse> next = pipeline;

            pipeline = () =>
                behaviour.Handle(
                    (TRequest)request,
                    next,
                    cancellationToken);
        }

        return await pipeline();
    }
}