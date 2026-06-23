namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record AuditEventListItemDto(
    Guid Id,
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
    DateTime OccurredAt,
    DateTime CreatedAt,
    bool HasBeforeJson,
    bool HasAfterJson,
    bool HasPayloadJson,
    bool HasMetadata,
    bool HasError,
    bool HasDetails
);
