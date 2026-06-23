using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.Shared;

namespace Dhole.AuditLogs.Application.AuditEvents.GetCorrelationHistory;

public sealed class GetCorrelationHistoryQueryHandler(IAuditEventRepository auditEvents)
    : IQueryHandler<GetCorrelationHistoryQuery, Result<IReadOnlyCollection<AuditEventListItemDto>>>
{
    public async Task<Result<IReadOnlyCollection<AuditEventListItemDto>>> HandleAsync(
        GetCorrelationHistoryQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (query.CorrelationId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyCollection<AuditEventListItemDto>>(
                AuditLogsErrors.InvalidCorrelationId
            );
        }

        var events = await auditEvents.GetByCorrelationIdAsync(
            query.CorrelationId,
            cancellationToken
        );

        return Result.Success(events);
    }
}
