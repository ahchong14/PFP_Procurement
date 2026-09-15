using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PFP.Application.Abstractions.Messaging;
using PFP.Application.Common.Behaviours;

namespace PFP.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            foreach (var type in assembly.GetTypes().Where(t => t is { IsAbstract: false, IsInterface: false }))
            {
                foreach (var implementedInterface in type.GetInterfaces())
                {
                    if (implementedInterface.IsGenericType &&
                        implementedInterface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    {
                        services.AddScoped(implementedInterface, type);
                    }
                }
            }

            services.AddScoped<ISender, Sender>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
