using CustomCodeFramework.Mongo.Abstractions;
using Dhole.AuditLogs.Application.Abstractions.Mongo;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Infrastructure.Mongo.Documents;

namespace Dhole.AuditLogs.Infrastructure.Mongo;

public sealed class AuditEventPayloadWriter(IMongoContext mongoContext) : IAuditEventPayloadWriter
{
    public Task WriteAsync(AuditEventDto auditEvent, CancellationToken cancellationToken = default)
    {
        var document = new AuditEventPayloadDocument
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
            OccurredAtUtc = auditEvent.OccurredAt,
            AuditCreatedAtUtc = auditEvent.CreatedAt,
            BeforeJson = auditEvent.BeforeJson,
            AfterJson = auditEvent.AfterJson,
            PayloadJson = auditEvent.PayloadJson,
            Metadata = auditEvent.Metadata,
            Details = auditEvent.Details,
            ErrorMessage = auditEvent.ErrorMessage,
            StackTrace = auditEvent.StackTrace,
            HasBeforeJson = !string.IsNullOrWhiteSpace(auditEvent.BeforeJson),
            HasAfterJson = !string.IsNullOrWhiteSpace(auditEvent.AfterJson),
            HasPayloadJson = !string.IsNullOrWhiteSpace(auditEvent.PayloadJson),
            HasMetadata = !string.IsNullOrWhiteSpace(auditEvent.Metadata),
            HasDetails = !string.IsNullOrWhiteSpace(auditEvent.Details),
            HasError = !string.IsNullOrWhiteSpace(auditEvent.ErrorMessage),
        };

        return mongoContext
            .GetCollection<AuditEventPayloadDocument>()
            .InsertOneAsync(document, cancellationToken: cancellationToken);
    }
}
