using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetAuditEventSummary;

public sealed class GetAuditEventSummaryQueryHandler(IAuditEventRepository auditEvents)
    : IQueryHandler<GetAuditEventSummaryQuery, AuditEventSummaryDto>
{
    public Task<AuditEventSummaryDto> HandleAsync(
        GetAuditEventSummaryQuery query,
        CancellationToken cancellationToken = default
    )
    {
        return auditEvents.GetSummaryAsync(
            query.SourceService,
            query.EntityType,
            query.EntityId,
            query.UserId,
            query.CorrelationId,
            query.Action,
            query.EventType,
            query.FromUtc,
            query.ToUtc,
            cancellationToken
        );
    }
}
