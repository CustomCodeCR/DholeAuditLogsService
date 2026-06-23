using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.Shared;

namespace Dhole.AuditLogs.Application.AuditEvents.GetAuditEventById;

public sealed class GetAuditEventByIdQueryHandler(IAuditEventRepository auditEvents)
    : IQueryHandler<GetAuditEventByIdQuery, Result<AuditEventDto>>
{
    public async Task<Result<AuditEventDto>> HandleAsync(
        GetAuditEventByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var auditEvent = await auditEvents.GetDetailAsync(query.AuditEventId, cancellationToken);

        if (auditEvent is null)
        {
            return Result.Failure<AuditEventDto>(AuditLogsErrors.AuditEventNotFound);
        }

        return Result.Success(auditEvent);
    }
}
