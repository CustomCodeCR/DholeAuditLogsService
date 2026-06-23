namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record EntityHistoryRequest(string EntityType, Guid EntityId);
