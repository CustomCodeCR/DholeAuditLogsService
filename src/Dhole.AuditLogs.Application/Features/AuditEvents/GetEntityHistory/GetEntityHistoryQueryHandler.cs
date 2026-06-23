using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.Shared;

namespace Dhole.AuditLogs.Application.AuditEvents.GetEntityHistory;

public sealed class GetEntityHistoryQueryHandler(IAuditEventRepository auditEvents)
    : IQueryHandler<GetEntityHistoryQuery, Result<IReadOnlyCollection<AuditEventListItemDto>>>
{
    public async Task<Result<IReadOnlyCollection<AuditEventListItemDto>>> HandleAsync(
        GetEntityHistoryQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(query.EntityType) || query.EntityId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyCollection<AuditEventListItemDto>>(
                AuditLogsErrors.InvalidEntity
            );
        }

        var events = await auditEvents.GetByEntityAsync(
            query.EntityType,
            query.EntityId,
            cancellationToken
        );

        return Result.Success(events);
    }
}
