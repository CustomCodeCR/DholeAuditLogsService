namespace Dhole.AuditLogs.Domain.AuditEvents.ValueObjects;

public sealed record AuditEventDetailJson(
    string? FieldName,
    string? OldValue,
    string? NewValue,
    string? Metadata
);
