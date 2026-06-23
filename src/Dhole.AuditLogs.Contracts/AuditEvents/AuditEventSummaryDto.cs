namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record AuditEventSummaryDto(
    long TotalEvents,
    long TotalErrors,
    long TotalAccessDenied,
    long TotalUsers,
    long TotalEntities,
    IReadOnlyCollection<AuditEventSourceServiceDto> SourceServices,
    IReadOnlyCollection<AuditEventActionDto> Actions
);
