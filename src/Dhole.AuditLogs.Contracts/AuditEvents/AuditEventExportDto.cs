namespace Dhole.AuditLogs.Contracts.AuditEvents;

public sealed record AuditEventExportDto(string FileName, string ContentType, byte[] Content);
