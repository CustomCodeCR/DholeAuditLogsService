using CustomCodeFramework.Messaging.Inbox;
using CustomCodeFramework.Postgres.EntityFramework.Configurations;
using CustomCodeFramework.Postgres.EntityFramework.DbContexts;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dhole.AuditLogs.Persistence.DbContexts;

public sealed class ServiceDbContext(DbContextOptions<ServiceDbContext> options)
    : AppDbContextBase(options)
{
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auditlogs");

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceDbContext).Assembly);

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}
