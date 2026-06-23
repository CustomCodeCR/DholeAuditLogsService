using CustomCodeFramework.Mongo.Abstractions;
using CustomCodeFramework.Mongo.Collections;

namespace Dhole.AuditLogs.Infrastructure.Mongo.Documents;

[MongoCollectionName("audit_event_payloads")]
public sealed class AuditEventPayloadDocument : IReadModel
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string AuditEventId { get; init; } = default!;

    public string EventId { get; init; } = default!;

    public string CorrelationId { get; init; } = default!;

    public string SourceService { get; init; } = default!;

    public string? EntityType { get; init; }

    public string? EntityId { get; init; }

    public string Action { get; init; } = default!;

    public string? EventType { get; init; }

    public string? UserId { get; init; }

    public string? UserName { get; init; }

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }

    public DateTime OccurredAtUtc { get; init; }

    public DateTime AuditCreatedAtUtc { get; init; }

    public string? BeforeJson { get; init; }

    public string? AfterJson { get; init; }

    public string? PayloadJson { get; init; }

    public string? Metadata { get; init; }

    public string? Details { get; init; }

    public string? ErrorMessage { get; init; }

    public string? StackTrace { get; init; }

    public bool HasBeforeJson { get; init; }

    public bool HasAfterJson { get; init; }

    public bool HasPayloadJson { get; init; }

    public bool HasMetadata { get; init; }

    public bool HasDetails { get; init; }

    public bool HasError { get; init; }

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}
