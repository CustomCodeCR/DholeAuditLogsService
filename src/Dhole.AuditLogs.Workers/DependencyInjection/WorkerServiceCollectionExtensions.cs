using CustomCodeFramework.Mongo.DependencyInjection;
using CustomCodeFramework.Redis.DependencyInjection;
using CustomCodeFramework.Redis.Streams.DependencyInjection;
using CustomCodeFramework.Workers.DependencyInjection;
using Dhole.AuditLogs.Application.Abstractions.Mongo;
using Dhole.AuditLogs.Infrastructure.Mongo;
using Dhole.AuditLogs.Worker.Streams;
using Dhole.AuditLogs.Worker.Workers;

namespace Dhole.AuditLogs.Worker.DependencyInjection;

public static class WorkerServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLogsWorker(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCustomCodeRedis(configuration);
        services.AddCustomCodeRedisStreams(configuration);
        services.AddCustomCodeRedisStreamConsumerBackgroundService();

        services.AddCustomCodeMongo(configuration);

        services.AddScoped<IAuditEventPayloadWriter, AuditEventPayloadWriter>();
        services.AddScoped<IAuditErrorDetailWriter, AuditErrorDetailWriter>();

        services.AddCustomCodeRedisStreamHandler<AuditEventStreamHandler>();
        services.AddCustomCodeRedisStreamHandler<AuditErrorStreamHandler>();

        services.AddCustomCodeWorkers(configuration);
        services.AddCustomCodePeriodicWorker<AuditLogsHealthCheckWorker>();

        return services;
    }
}
