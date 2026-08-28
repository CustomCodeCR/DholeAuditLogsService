namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record AuditEventDto(
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
    string? BeforeJson,
    string? AfterJson,
    string? PayloadJson,
    string? Metadata,
    string? ErrorMessage,
    string? StackTrace,
    string? Details
);
