using CustomCodeFramework.Messaging.Inbox;
using CustomCodeFramework.Postgres.DependencyInjection;
using CustomCodeFramework.Postgres.EntityFramework.DependencyInjection;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Persistence.DbContexts;
using Dhole.AuditLogs.Persistence.Messaging;
using Dhole.AuditLogs.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dhole.AuditLogs.Persistence.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCustomCodePostgres(configuration);
        services.AddCustomCodePostgresEntityFramework<ServiceDbContext>();

        services.AddScoped<IAuditEventRepository, AuditEventRepository>();

        services.AddScoped<IInboxStore, InboxStore>();

        return services;
    }
}
