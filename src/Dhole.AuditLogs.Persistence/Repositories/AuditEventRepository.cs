using CustomCodeFramework.Core.Pagination;
using CustomCodeFramework.Postgres.EntityFramework.Repositories;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;
using Dhole.AuditLogs.Domain.Shared;
using Dhole.AuditLogs.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Dhole.AuditLogs.Persistence.Repositories;

public sealed class AuditEventRepository(ServiceDbContext dbContext)
    : EfRepository<AuditEvent, Guid>(dbContext),
        IAuditEventRepository
{
    public Task<AuditEvent?> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.AuditEvents.FirstOrDefaultAsync(
            x => x.EventId == eventId,
            cancellationToken
        );
    }

    public async Task<AuditEventDto?> GetDetailAsync(
        Guid auditEventId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .AuditEvents.AsNoTracking()
            .Where(x => x.Id == auditEventId)
            .Select(x => new AuditEventDto(
                x.Id,
                x.EventId,
                x.CorrelationId,
                x.SourceService,
                x.EntityType,
                x.EntityId,
                x.Action,
                x.EventType,
                x.UserId,
                x.UserName,
                x.IpAddress,
                x.UserAgent,
                x.OccurredAt,
                x.CreatedAt,
                x.BeforeJson,
                x.AfterJson,
                x.PayloadJson,
                x.MetadataJson,
                x.ErrorMessage,
                x.StackTrace,
                x.DetailsJson
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<AuditEventListItemDto>> GetPagedAsync(
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
    )
    {
        var query = ApplyFilters(
            dbContext.AuditEvents.AsNoTracking(),
            sourceService,
            entityType,
            entityId,
            userId,
            correlationId,
            action,
            eventType,
            fromUtc,
            toUtc
        );

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.CreatedAt)
            .Skip(page.Skip)
            .Take(page.PageSize)
            .Select(x => new AuditEventListItemDto(
                x.Id,
                x.EventId,
                x.CorrelationId,
                x.SourceService,
                x.EntityType,
                x.EntityId,
                x.Action,
                x.EventType,
                x.UserId,
                x.UserName,
                x.IpAddress,
                x.OccurredAt,
                x.CreatedAt,
                x.BeforeJson != null,
                x.AfterJson != null,
                x.PayloadJson != null,
                x.MetadataJson != null,
                !string.IsNullOrEmpty(x.ErrorMessage),
                x.DetailsJson != null
            ))
            .ToListAsync(cancellationToken);

        return PagedResult<AuditEventListItemDto>.Create(
            items,
            page.PageNumber,
            page.PageSize,
            total
        );
    }

    public async Task<IReadOnlyCollection<AuditEventListItemDto>> GetByEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default
    )
    {
        var value = entityType.Trim();

        return await dbContext
            .AuditEvents.AsNoTracking()
            .Where(x => x.EntityType == value && x.EntityId == entityId)
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new AuditEventListItemDto(
                x.Id,
                x.EventId,
                x.CorrelationId,
                x.SourceService,
                x.EntityType,
                x.EntityId,
                x.Action,
                x.EventType,
                x.UserId,
                x.UserName,
                x.IpAddress,
                x.OccurredAt,
                x.CreatedAt,
                x.BeforeJson != null,
                x.AfterJson != null,
                x.PayloadJson != null,
                x.MetadataJson != null,
                !string.IsNullOrEmpty(x.ErrorMessage),
                x.DetailsJson != null
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditEventListItemDto>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .AuditEvents.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new AuditEventListItemDto(
                x.Id,
                x.EventId,
                x.CorrelationId,
                x.SourceService,
                x.EntityType,
                x.EntityId,
                x.Action,
                x.EventType,
                x.UserId,
                x.UserName,
                x.IpAddress,
                x.OccurredAt,
                x.CreatedAt,
                x.BeforeJson != null,
                x.AfterJson != null,
                x.PayloadJson != null,
                x.MetadataJson != null,
                !string.IsNullOrEmpty(x.ErrorMessage),
                x.DetailsJson != null
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditEventListItemDto>> GetByCorrelationIdAsync(
        Guid correlationId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .AuditEvents.AsNoTracking()
            .Where(x => x.CorrelationId == correlationId)
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new AuditEventListItemDto(
                x.Id,
                x.EventId,
                x.CorrelationId,
                x.SourceService,
                x.EntityType,
                x.EntityId,
                x.Action,
                x.EventType,
                x.UserId,
                x.UserName,
                x.IpAddress,
                x.OccurredAt,
                x.CreatedAt,
                x.BeforeJson != null,
                x.AfterJson != null,
                x.PayloadJson != null,
                x.MetadataJson != null,
                !string.IsNullOrEmpty(x.ErrorMessage),
                x.DetailsJson != null
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<AuditEventSummaryDto> GetSummaryAsync(
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
    )
    {
        var query = ApplyFilters(
            dbContext.AuditEvents.AsNoTracking(),
            sourceService,
            entityType,
            entityId,
            userId,
            correlationId,
            action,
            eventType,
            fromUtc,
            toUtc
        );

        var totalEvents = await query.LongCountAsync(cancellationToken);

        var totalErrors = await query.LongCountAsync(
            x =>
                x.Action == AuditLogsConstants.Actions.Error
                || !string.IsNullOrEmpty(x.ErrorMessage),
            cancellationToken
        );

        var totalAccessDenied = await query.LongCountAsync(
            x =>
                x.Action == AuditLogsConstants.Actions.AccessDenied
                || x.Action == AuditLogsConstants.Results.Denied,
            cancellationToken
        );

        var totalUsers = await query
            .Where(x => x.UserId.HasValue)
            .Select(x => x.UserId)
            .Distinct()
            .LongCountAsync(cancellationToken);

        var totalEntities = await query
            .Where(x => x.EntityType != null && x.EntityId.HasValue)
            .Select(x => new { x.EntityType, x.EntityId })
            .Distinct()
            .LongCountAsync(cancellationToken);

        var sourceServices = await query
            .GroupBy(x => x.SourceService)
            .Select(x => new AuditEventSourceServiceDto(x.Key, x.LongCount()))
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.SourceService)
            .ToListAsync(cancellationToken);

        var actions = await query
            .GroupBy(x => x.Action)
            .Select(x => new AuditEventActionDto(x.Key, x.LongCount()))
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Action)
            .ToListAsync(cancellationToken);

        return new AuditEventSummaryDto(
            totalEvents,
            totalErrors,
            totalAccessDenied,
            totalUsers,
            totalEntities,
            sourceServices,
            actions
        );
    }

    private static IQueryable<AuditEvent> ApplyFilters(
        IQueryable<AuditEvent> query,
        string? sourceService,
        string? entityType,
        Guid? entityId,
        Guid? userId,
        Guid? correlationId,
        string? action,
        string? eventType,
        DateTime? fromUtc,
        DateTime? toUtc
    )
    {
        if (!string.IsNullOrWhiteSpace(sourceService))
        {
            var value = sourceService.Trim().ToLower();

            query = query.Where(x => x.SourceService.ToLower().Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            var value = entityType.Trim();

            query = query.Where(x => x.EntityType == value);
        }

        if (entityId.HasValue)
        {
            query = query.Where(x => x.EntityId == entityId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        if (correlationId.HasValue)
        {
            query = query.Where(x => x.CorrelationId == correlationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            var value = action.Trim();

            query = query.Where(x => x.Action == value);
        }

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            var value = eventType.Trim();

            query = query.Where(x => x.EventType == value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.OccurredAt >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.OccurredAt <= toUtc.Value);
        }

        return query;
    }
}
