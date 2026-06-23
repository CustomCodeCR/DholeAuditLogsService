using CustomCodeFramework.Mongo.Abstractions;
using Dhole.AuditLogs.Application.Abstractions.Mongo;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Infrastructure.Mongo.Documents;

namespace Dhole.AuditLogs.Infrastructure.Mongo;

public sealed class AuditErrorDetailWriter(IMongoContext mongoContext) : IAuditErrorDetailWriter
{
    public Task WriteAsync(AuditEventDto auditEvent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(auditEvent.ErrorMessage))
        {
            return Task.CompletedTask;
        }

        var document = new AuditErrorDetailDocument
        {
            AuditEventId = auditEvent.Id.ToString(),
            EventId = auditEvent.EventId.ToString(),
            CorrelationId = auditEvent.CorrelationId.ToString(),
            SourceService = auditEvent.SourceService,
            EntityType = auditEvent.EntityType,
            EntityId = auditEvent.EntityId?.ToString(),
            Action = auditEvent.Action,
            EventType = auditEvent.EventType,
            UserId = auditEvent.UserId?.ToString(),
            UserName = auditEvent.UserName,
            IpAddress = auditEvent.IpAddress,
            UserAgent = auditEvent.UserAgent,
            ErrorMessage = auditEvent.ErrorMessage,
            StackTrace = auditEvent.StackTrace,
            Metadata = auditEvent.Metadata,
            PayloadJson = auditEvent.PayloadJson,
            Details = auditEvent.Details,
            OccurredAtUtc = auditEvent.OccurredAt,
            AuditCreatedAtUtc = auditEvent.CreatedAt,
        };

        return mongoContext
            .GetCollection<AuditErrorDetailDocument>()
            .InsertOneAsync(document, cancellationToken: cancellationToken);
    }
}
