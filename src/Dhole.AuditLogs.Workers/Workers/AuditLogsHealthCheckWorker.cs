using CustomCodeFramework.Messaging.Inbox;
using CustomCodeFramework.Workers.Abstractions;
using Dhole.AuditLogs.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Dhole.AuditLogs.Worker.Workers;

internal sealed class AuditLogsHealthCheckWorker(
    ServiceDbContext dbContext,
    ILogger<AuditLogsHealthCheckWorker> logger
) : IBackgroundWorker
{
    public string Name => "auditlogs.health-check";

    public async Task ExecuteAsync(
        IWorkerExecutionContext context,
        CancellationToken cancellationToken
    )
    {
        var auditEventsCount = await dbContext.AuditEvents.CountAsync(cancellationToken);

        var inboxMessagesCount = await dbContext.InboxMessages.CountAsync(cancellationToken);

        var pendingInboxMessagesCount = await dbContext.InboxMessages.CountAsync(
            x => x.Status != InboxMessageStatus.Processed || x.ProcessedAtUtc == null,
            cancellationToken
        );

        logger.LogInformation(
            "AuditLogs health check. AuditEvents: {AuditEventsCount}, InboxMessages: {InboxMessagesCount}, PendingInbox: {PendingInboxMessagesCount}.",
            auditEventsCount,
            inboxMessagesCount,
            pendingInboxMessagesCount
        );
    }
}
