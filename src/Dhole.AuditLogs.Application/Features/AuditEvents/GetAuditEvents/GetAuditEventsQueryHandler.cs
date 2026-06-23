using CustomCodeFramework.Core.Pagination;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetAuditEvents;

public sealed class GetAuditEventsQueryHandler(IAuditEventRepository auditEvents)
    : IQueryHandler<GetAuditEventsQuery, PagedResult<AuditEventListItemDto>>
{
    public Task<PagedResult<AuditEventListItemDto>> HandleAsync(
        GetAuditEventsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        return auditEvents.GetPagedAsync(
            query.Page,
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
