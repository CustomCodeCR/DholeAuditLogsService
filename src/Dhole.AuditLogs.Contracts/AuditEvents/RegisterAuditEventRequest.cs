namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record RegisterAuditEventRequest(
    Guid EventId,
    Guid CorrelationId,
    string SourceService,
    string? EntityType,
    Guid? EntityId,
    string Action,
    string? EventType,
    Guid? UserId,
    string? UserName,
    string? IpAddress,
    string? UserAgent,
    DateTime OccurredAt,
    string? BeforeJson,
    string? AfterJson,
    string? PayloadJson,
    string? Metadata,
    string? ErrorMessage,
    string? StackTrace,
    IReadOnlyCollection<RegisterAuditEventDetailRequest>? Details
);
