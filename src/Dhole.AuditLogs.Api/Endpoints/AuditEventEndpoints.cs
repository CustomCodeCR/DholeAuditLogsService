using CustomCodeFramework.Api.Responses;
using CustomCodeFramework.Core.Pagination;
using CustomCodeFramework.Cqrs.Dispatching;
using Dhole.AuditLogs.Api.Authorization;
using Dhole.AuditLogs.Application.AuditEvents.GetAuditEventById;
using Dhole.AuditLogs.Application.AuditEvents.GetAuditEvents;
using Dhole.AuditLogs.Application.AuditEvents.GetAuditEventSummary;
using Dhole.AuditLogs.Application.AuditEvents.GetCorrelationHistory;
using Dhole.AuditLogs.Application.AuditEvents.GetEntityHistory;
using Dhole.AuditLogs.Application.AuditEvents.GetUserHistory;
using Dhole.AuditLogs.Contracts.AuditEvents;

namespace Dhole.AuditLogs.Api.Endpoints;

public static class AuditEventEndpoints
{
    public static IEndpointRouteBuilder MapAuditEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auditlogs/events")
            .WithTags("Audit Logs")
            .RequireAuthorization();

        group
            .MapGet(
                "/",
                async (
                    int pageNumber,
                    int pageSize,
                    string? sourceService,
                    string? entityType,
                    Guid? entityId,
                    Guid? userId,
                    Guid? correlationId,
                    string? action,
                    string? eventType,
                    DateTime? fromUtc,
                    DateTime? toUtc,
                    IQueryDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await dispatcher.DispatchAsync(
                        new GetAuditEventsQuery(
                            PageRequest.Create(pageNumber, pageSize),
                            sourceService,
                            entityType,
                            entityId,
                            userId,
                            correlationId,
                            action,
                            eventType,
                            fromUtc,
                            toUtc
                        ),
                        cancellationToken
                    );

                    return EndpointResults.FromPaged(result);
                }
            )
            .RequireScope(AuditLogsScopeNames.EventsView);

        group
            .MapGet(
                "/summary",
                async (
                    string? sourceService,
                    string? entityType,
                    Guid? entityId,
                    Guid? userId,
                    Guid? correlationId,
                    string? action,
                    string? eventType,
                    DateTime? fromUtc,
                    DateTime? toUtc,
                    IQueryDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await dispatcher.DispatchAsync(
                        new GetAuditEventSummaryQuery(
                            sourceService,
                            entityType,
                            entityId,
                            userId,
                            correlationId,
                            action,
                            eventType,
                            fromUtc,
                            toUtc
                        ),
                        cancellationToken
                    );

                    return Results.Ok(ApiResponse<AuditEventSummaryDto>.Ok(result));
                }
            )
            .RequireScope(AuditLogsScopeNames.EventsView);

        group
            .MapGet(
                "/entity-history",
                async (
                    string entityType,
                    Guid entityId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await dispatcher.DispatchAsync(
                        new GetEntityHistoryQuery(entityType, entityId),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.EntityHistoryView);

        group
            .MapGet(
                "/user-history/{userId:guid}",
                async (
                    Guid userId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await dispatcher.DispatchAsync(
                        new GetUserHistoryQuery(userId),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.UserHistoryView);

        group
            .MapGet(
                "/correlation-history/{correlationId:guid}",
                async (
                    Guid correlationId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await dispatcher.DispatchAsync(
                        new GetCorrelationHistoryQuery(correlationId),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.EventsView);

        group
            .MapGet(
                "/{auditEventId:guid}",
                async (
                    Guid auditEventId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await dispatcher.DispatchAsync(
                        new GetAuditEventByIdQuery(auditEventId),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.EventsView);

        return app;
    }
}
