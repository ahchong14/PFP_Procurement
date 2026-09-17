using PFP.Application.Abstractions.Messaging;
using System.Collections.Concurrent;

namespace PFP.Application.Internal.Messaging;

public sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> HandlerCache = new();

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        Type requestType = request.GetType();

        RequestHandlerBase wrapper = HandlerCache.GetOrAdd(requestType, reqType =>
        {
            Type wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(reqType, typeof(TResponse));
            return (RequestHandlerBase)Activator.CreateInstance(wrapperType)!;
        });

        object? result = await wrapper.Handle(request, serviceProvider, cancellationToken);
        return (TResponse)result!;
    }
}
