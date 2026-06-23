using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetAuditEventById;

public sealed record GetAuditEventByIdQuery(Guid AuditEventId) : IQuery<Result<AuditEventDto>>;
