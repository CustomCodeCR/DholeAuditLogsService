using CustomCodeFramework.Core.Pagination;
using CustomCodeFramework.Cqrs.Dispatching;
using Dhole.AuditLogs.Api.Authorization;
using Dhole.AuditLogs.Application.AuditEvents.GetAuditEventById;
using Dhole.AuditLogs.Application.AuditEvents.GetAuditEvents;
using Dhole.AuditLogs.Application.AuditEvents.GetAuditEventSummary;
using Dhole.AuditLogs.Application.AuditEvents.GetCorrelationHistory;
using Dhole.AuditLogs.Application.AuditEvents.GetEntityHistory;
using Dhole.AuditLogs.Application.AuditEvents.GetUserHistory;

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
                    string? entityId,
                    string? userId,
                    string? correlationId,
                    string? action,
                    string? eventType,
                    DateTime? fromUtc,
                    DateTime? toUtc,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var parsedEntityId = ParseNullableGuid(entityId);
                    var parsedUserId = ParseNullableGuid(userId);
                    var parsedCorrelationId = ParseNullableGuid(correlationId);

                    if (!parsedEntityId.IsValid)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidEntityId",
                            "El filtro entityId no es un Guid válido.",
                            httpContext
                        );
                    }

                    if (!parsedUserId.IsValid)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidUserId",
                            "El filtro userId no es un Guid válido.",
                            httpContext
                        );
                    }

                    if (!parsedCorrelationId.IsValid)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidCorrelationId",
                            "El filtro correlationId no es un Guid válido.",
                            httpContext
                        );
                    }

                    var result = await dispatcher.DispatchAsync(
                        new GetAuditEventsQuery(
                            PageRequest.Create(pageNumber, pageSize),
                            sourceService,
                            entityType,
                            parsedEntityId.Value,
                            parsedUserId.Value,
                            parsedCorrelationId.Value,
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
                    string? entityId,
                    string? userId,
                    string? correlationId,
                    string? action,
                    string? eventType,
                    DateTime? fromUtc,
                    DateTime? toUtc,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var parsedEntityId = ParseNullableGuid(entityId);
                    var parsedUserId = ParseNullableGuid(userId);
                    var parsedCorrelationId = ParseNullableGuid(correlationId);

                    if (!parsedEntityId.IsValid)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidEntityId",
                            "El filtro entityId no es un Guid válido.",
                            httpContext
                        );
                    }

                    if (!parsedUserId.IsValid)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidUserId",
                            "El filtro userId no es un Guid válido.",
                            httpContext
                        );
                    }

                    if (!parsedCorrelationId.IsValid)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidCorrelationId",
                            "El filtro correlationId no es un Guid válido.",
                            httpContext
                        );
                    }

                    var result = await dispatcher.DispatchAsync(
                        new GetAuditEventSummaryQuery(
                            sourceService,
                            entityType,
                            parsedEntityId.Value,
                            parsedUserId.Value,
                            parsedCorrelationId.Value,
                            action,
                            eventType,
                            fromUtc,
                            toUtc
                        ),
                        cancellationToken
                    );

                    return EndpointResults.Ok(result);
                }
            )
            .RequireScope(AuditLogsScopeNames.EventsView);

        group
            .MapGet(
                "/entity-history",
                async (
                    string entityType,
                    string entityId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var parsedEntityId = ParseNullableGuid(entityId);

                    if (!parsedEntityId.IsValid || parsedEntityId.Value is null)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidEntityId",
                            "El entityId no es un Guid válido.",
                            httpContext
                        );
                    }

                    var result = await dispatcher.DispatchAsync(
                        new GetEntityHistoryQuery(entityType, parsedEntityId.Value.Value),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.EntityHistoryView);

        group
            .MapGet(
                "/user-history/{userId}",
                async (
                    string userId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var parsedUserId = ParseNullableGuid(userId);

                    if (!parsedUserId.IsValid || parsedUserId.Value is null)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidUserId",
                            "El userId no es un Guid válido.",
                            httpContext
                        );
                    }

                    var result = await dispatcher.DispatchAsync(
                        new GetUserHistoryQuery(parsedUserId.Value.Value),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.UserHistoryView);

        group
            .MapGet(
                "/correlation-history/{correlationId}",
                async (
                    string correlationId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var parsedCorrelationId = ParseNullableGuid(correlationId);

                    if (!parsedCorrelationId.IsValid || parsedCorrelationId.Value is null)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidCorrelationId",
                            "El correlationId no es un Guid válido.",
                            httpContext
                        );
                    }

                    var result = await dispatcher.DispatchAsync(
                        new GetCorrelationHistoryQuery(parsedCorrelationId.Value.Value),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.EventsView);

        group
            .MapGet(
                "/{auditEventId}",
                async (
                    string auditEventId,
                    IQueryDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    var parsedAuditEventId = ParseNullableGuid(auditEventId);

                    if (!parsedAuditEventId.IsValid || parsedAuditEventId.Value is null)
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidAuditEventId",
                            "El auditEventId no es un Guid válido.",
                            httpContext
                        );
                    }

                    var result = await dispatcher.DispatchAsync(
                        new GetAuditEventByIdQuery(parsedAuditEventId.Value.Value),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .RequireScope(AuditLogsScopeNames.EventsView);

        return app;
    }

    private static NullableGuidParseResult ParseNullableGuid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new NullableGuidParseResult(true, null);
        }

        var normalized = value.Trim().Trim('"').Trim('\'');

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return new NullableGuidParseResult(true, null);
        }

        return Guid.TryParse(normalized, out var guid)
            ? new NullableGuidParseResult(true, guid)
            : new NullableGuidParseResult(false, null);
    }

    private sealed record NullableGuidParseResult(bool IsValid, Guid? Value);
}
