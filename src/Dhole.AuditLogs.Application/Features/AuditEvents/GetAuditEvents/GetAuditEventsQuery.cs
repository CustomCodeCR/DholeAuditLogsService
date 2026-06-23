using CustomCodeFramework.Core.Pagination;
using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetAuditEvents;

public sealed record GetAuditEventsQuery(
    PageRequest Page,
    string? SourceService,
    string? EntityType,
    Guid? EntityId,
    Guid? UserId,
    Guid? CorrelationId,
    string? Action,
    string? EventType,
    DateTime? FromUtc,
    DateTime? ToUtc
) : IQuery<PagedResult<AuditEventListItemDto>>;
