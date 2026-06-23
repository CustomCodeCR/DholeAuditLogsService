namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record AuditEventDetailDto(
    string? FieldName,
    string? OldValue,
    string? NewValue,
    string? Metadata
);
