using CustomCodeFramework.Core.Pagination;
using CustomCodeFramework.Persistence.Abstractions;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;

namespace Dhole.AuditLogs.Application.Abstractions.Repositories;

public interface IAuditEventRepository : IRepository<AuditEvent, Guid>
{
    Task<AuditEvent?> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default
    );

    Task<AuditEventDto?> GetDetailAsync(
        Guid auditEventId,
        CancellationToken cancellationToken = default
    );

    Task<PagedResult<AuditEventListItemDto>> GetPagedAsync(
        PageRequest page,
        string? sourceService = null,
        string? entityType = null,
        Guid? entityId = null,
        Guid? userId = null,
        Guid? correlationId = null,
        string? action = null,
        string? eventType = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<AuditEventListItemDto>> GetByEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<AuditEventListItemDto>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<AuditEventListItemDto>> GetByCorrelationIdAsync(
        Guid correlationId,
        CancellationToken cancellationToken = default
    );

    Task<AuditEventSummaryDto> GetSummaryAsync(
        string? sourceService = null,
        string? entityType = null,
        Guid? entityId = null,
        Guid? userId = null,
        Guid? correlationId = null,
        string? action = null,
        string? eventType = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default
    );
}
