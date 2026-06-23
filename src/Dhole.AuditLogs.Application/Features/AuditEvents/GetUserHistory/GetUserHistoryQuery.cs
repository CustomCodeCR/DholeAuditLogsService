using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetUserHistory;

public sealed record GetUserHistoryQuery(Guid UserId)
    : IQuery<Result<IReadOnlyCollection<AuditEventListItemDto>>>;
