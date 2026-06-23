using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.Shared;

namespace Dhole.AuditLogs.Application.AuditEvents.GetUserHistory;

public sealed class GetUserHistoryQueryHandler(IAuditEventRepository auditEvents)
    : IQueryHandler<GetUserHistoryQuery, Result<IReadOnlyCollection<AuditEventListItemDto>>>
{
    public async Task<Result<IReadOnlyCollection<AuditEventListItemDto>>> HandleAsync(
        GetUserHistoryQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (query.UserId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyCollection<AuditEventListItemDto>>(
                AuditLogsErrors.InvalidUser
            );
        }

        var events = await auditEvents.GetByUserAsync(query.UserId, cancellationToken);

        return Result.Success(events);
    }
}
