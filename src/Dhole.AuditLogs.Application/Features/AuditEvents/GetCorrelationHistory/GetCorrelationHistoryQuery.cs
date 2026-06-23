using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetCorrelationHistory;

public sealed record GetCorrelationHistoryQuery(Guid CorrelationId)
    : IQuery<Result<IReadOnlyCollection<AuditEventListItemDto>>>;
