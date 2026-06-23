using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetEntityHistory;

public sealed record GetEntityHistoryQuery(string EntityType, Guid EntityId)
    : IQuery<Result<IReadOnlyCollection<AuditEventListItemDto>>>;
