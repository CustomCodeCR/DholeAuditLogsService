namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record RegisterAuditEventDetailRequest(
    string? FieldName,
    string? OldValue,
    string? NewValue,
    string? Metadata
);
