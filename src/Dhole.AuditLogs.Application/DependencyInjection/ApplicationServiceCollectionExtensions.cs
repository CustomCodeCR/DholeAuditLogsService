using CustomCodeFramework.Cqrs.DependencyInjection;
using CustomCodeFramework.Validation.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dhole.AuditLogs.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCustomCodeValidation(AssemblyReference.Assembly);

        services.AddCustomCodeCqrs(AssemblyReference.Assembly);
        services.AddCustomCodeCqrsBehaviors();

        return services;
    }
}
