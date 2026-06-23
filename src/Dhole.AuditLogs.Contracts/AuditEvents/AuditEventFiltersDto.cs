namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record AuditEventFiltersDto(
    string? SourceService,
    string? EntityType,
    Guid? EntityId,
    Guid? UserId,
    Guid? CorrelationId,
    string? Action,
    string? EventType,
    DateTime? FromUtc,
    DateTime? ToUtc
);
