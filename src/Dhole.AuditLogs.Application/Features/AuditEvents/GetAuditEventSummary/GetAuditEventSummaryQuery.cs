using CustomCodeFramework.Cqrs.Queries;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Application.AuditEvents.GetAuditEventSummary;

public sealed record GetAuditEventSummaryQuery(
    string? SourceService,
    string? EntityType,
    Guid? EntityId,
    Guid? UserId,
    Guid? CorrelationId,
    string? Action,
    string? EventType,
    DateTime? FromUtc,
    DateTime? ToUtc
) : IQuery<AuditEventSummaryDto>;
