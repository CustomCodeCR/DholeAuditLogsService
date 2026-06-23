using CustomCodeFramework.Auth.DependencyInjection;
using CustomCodeFramework.Mongo.DependencyInjection;
using Dhole.AuditLogs.Application.Abstractions.Mongo;
using Dhole.AuditLogs.Infrastructure.Mongo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dhole.AuditLogs.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCustomCodeAuth(configuration);

        services.PostConfigure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        services.AddCustomCodeMongo(configuration);

        services.AddScoped<IAuditEventPayloadWriter, AuditEventPayloadWriter>();
        services.AddScoped<IAuditErrorDetailWriter, AuditErrorDetailWriter>();

        return services;
    }
}
