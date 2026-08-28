namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record RegisterAuditAccessRequest(
    string PageName,
    string Route,
    string? ResourceType = null,
    string? ResourceId = null
);
