namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record AuditEventListItemDto(
    Guid Id,
    Guid EventId,
    Guid CorrelationId,
    string SourceService,
    string? EntityType,
    Guid? EntityId,
    string? EntityName,
    string Action,
    string? EventType,
    Guid? UserId,
    string? UserName,
    string? IpAddress,
    string? UserAgent,
    DateTime OccurredAt,
    DateTime CreatedAt,
    string? Description,
    string? HttpMethod,
    string? RequestPath,
    bool HasBeforeJson,
    bool HasAfterJson,
    bool HasPayloadJson,
    bool HasMetadata,
    bool HasError,
    bool HasDetails
);
