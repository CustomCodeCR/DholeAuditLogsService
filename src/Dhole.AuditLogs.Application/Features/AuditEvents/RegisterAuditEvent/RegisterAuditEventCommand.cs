using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Commands;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.RegisterAuditEvent;

public sealed record RegisterAuditEventCommand(RegisterAuditEventRequest Request)
    : ICommand<Result<Guid>>;
