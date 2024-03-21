using System.Reflection;
using Core.PreProcessors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Singleton);

        return services
            .AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                conf.AddOpenRequestPreProcessor(typeof(CommandValidationPreProcessor<>),
                    serviceLifetime: ServiceLifetime.Scoped);
            });
    }
}