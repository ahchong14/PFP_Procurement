using System.Reflection;
using PFP.Application.Abstractions.Messaging;
using PFP.Application.Abstractions.Services;
using PFP.Application.Common.Authorization;
using PFP.Application.Common.Exceptions;

namespace PFP.Application.Common.Behaviours
{
    public sealed class AuthorizationBehaviour<TRequest, TResponse>(
        ICurrentUserService currentUserService)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public Task<TResponse> Handle(
            TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var attribute = typeof(TRequest).GetCustomAttribute<RequireRoleAttribute>();

            if (attribute is not null)
            {
                if (!currentUserService.IsAuthenticated)
                    throw new UnauthorizedException("User is not authenticated.");

                var currentRole = currentUserService.Role;

                if (currentRole is null || !attribute.AllowedRoles.Contains(currentRole.Value))
                    throw new ForbiddenException($"Role \"{currentRole}\" is not allowed to perform this action.");
            }

            return next();
        }
    }
}
