using CustomCodeFramework.Messaging.Inbox;
using CustomCodeFramework.Messaging.Outbox;
using CustomCodeFramework.Postgres.EntityFramework.Configurations;
using CustomCodeFramework.Postgres.EntityFramework.DbContexts;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Dhole.AuditLogs.Persistence.Messaging;

namespace Dhole.AuditLogs.Persistence.DbContexts;

public sealed class ServiceDbContext(DbContextOptions<ServiceDbContext> options,
    IConfiguration? realtimeConfiguration = null)
    : AppDbContextBase(options)
{
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AppendRealtimeAuditInvalidations();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        AppendRealtimeAuditInvalidations();
        return base.SaveChanges();
    }

    private void AppendRealtimeAuditInvalidations()
    {
        if (!AuditRealtimeInvalidation.IsEnabled(realtimeConfiguration))
            return;
        ChangeTracker.DetectChanges();
        foreach (var eventEntry in ChangeTracker.Entries<AuditEvent>()
            .Where(x => x.State == EntityState.Added).ToArray())
        {
            var auditId = eventEntry.Entity.Id;
            if (auditId == Guid.Empty) continue;
            var id = Guid.NewGuid();
            var now = DateTimeOffset.UtcNow;
            var correlationId = eventEntry.Entity.CorrelationId == Guid.Empty
                ? id.ToString("D") : eventEntry.Entity.CorrelationId.ToString("D");
            OutboxMessages.Add(new OutboxMessage
            {
                EventId = id,
                EventType = AuditRealtimeInvalidation.Type,
                EventName = AuditRealtimeInvalidation.Type,
                SourceService = "DholeAuditLogsService",
                PayloadJson = AuditRealtimeInvalidation.Serialize(
                    id, auditId, AuditRealtimeInvalidation.NextVersionHint(),
                    correlationId, now),
                CorrelationId = correlationId,
                Status = OutboxMessageStatus.Pending,
                RetryCount = 0,
                CreatedAtUtc = now.UtcDateTime
            });
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auditlogs");

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceDbContext).Assembly);

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
