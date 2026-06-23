using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.Abstractions.Mongo;

public interface IAuditEventPayloadWriter
{
    Task WriteAsync(AuditEventDto auditEvent, CancellationToken cancellationToken = default);
}
