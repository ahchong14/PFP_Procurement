namespace PFP.Application.Abstractions.Messaging
{
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
}
