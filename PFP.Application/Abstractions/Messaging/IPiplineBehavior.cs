namespace PFP.Application.Abstractions.Messaging;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();   // ← 这一行

public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
