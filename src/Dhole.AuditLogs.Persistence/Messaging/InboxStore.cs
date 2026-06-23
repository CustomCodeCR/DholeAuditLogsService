using CustomCodeFramework.Messaging.Inbox;
using Dhole.AuditLogs.Domain.Shared;
using Dhole.AuditLogs.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Dhole.AuditLogs.Persistence.Messaging;

public sealed class InboxStore(ServiceDbContext dbContext) : IInboxStore
{
    private const string ConsumerService = AuditLogsConstants.ServiceName;

    public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return dbContext.InboxMessages.AnyAsync(
            x => x.EventId == eventId && x.ConsumerService == ConsumerService,
            cancellationToken
        );
    }

    public async Task AddAsync(
        Guid eventId,
        string eventName,
        DateTime processedAtUtc,
        CancellationToken cancellationToken = default
    )
    {
        var message = new InboxMessage
        {
            EventId = eventId,
            EventType = eventName,
            EventName = eventName,
            SourceService = AuditLogsConstants.Streams.AuditEvents,
            ConsumerService = ConsumerService,
            CorrelationId = null,
            Status = InboxMessageStatus.Processed,
            ProcessedAtUtc = processedAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
        };

        await dbContext.InboxMessages.AddAsync(message, cancellationToken);
    }
}
