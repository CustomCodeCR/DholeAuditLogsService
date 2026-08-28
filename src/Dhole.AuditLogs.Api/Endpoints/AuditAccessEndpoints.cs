using System.Security.Claims;
using System.Text.Json;
using CustomCodeFramework.Cqrs.Dispatching;
using Dhole.AuditLogs.Application.AuditEvents.RegisterAuditEvent;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.Shared;

namespace Dhole.AuditLogs.Api.Endpoints;

public static class AuditAccessEndpoints
{
    public static IEndpointRouteBuilder MapAuditAccessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/auditlogs/access",
                async (
                    RegisterAuditAccessRequest request,
                    ICommandDispatcher dispatcher,
                    HttpContext httpContext,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (string.IsNullOrWhiteSpace(request.PageName) || string.IsNullOrWhiteSpace(request.Route))
                    {
                        return EndpointResults.BadRequest(
                            "AuditLogs.InvalidAccessEvent",
                            "La pantalla y la ruta son obligatorias para registrar la visualización.",
                            httpContext
                        );
                    }

                    var userId = ResolveUserId(httpContext.User);
                    var userName = ResolveUserName(httpContext.User);
                    var entityId = Guid.TryParse(request.ResourceId, out var parsedEntityId)
                        ? parsedEntityId
                        : (Guid?)null;
                    var entityType = string.IsNullOrWhiteSpace(request.ResourceType)
                        ? "Screen"
                        : request.ResourceType.Trim();
                    var pageName = request.PageName.Trim();
                    var route = request.Route.Trim();
                    var metadata = JsonSerializer.Serialize(
                        new
                        {
                            pageName,
                            route,
                            resourceType = request.ResourceType,
                            resourceId = request.ResourceId,
                        }
                    );

                    var auditRequest = new RegisterAuditEventRequest(
                        EventId: Guid.NewGuid(),
                        CorrelationId: Guid.NewGuid(),
                        SourceService: "DholeWeb",
                        EntityType: entityType,
                        EntityId: entityId,
                        Action: AuditLogsConstants.Actions.Viewed,
                        EventType: "ui.page.viewed",
                        UserId: userId,
                        UserName: userName,
                        IpAddress: ResolveIpAddress(httpContext),
                        UserAgent: httpContext.Request.Headers.UserAgent.ToString(),
                        OccurredAt: DateTime.UtcNow,
                        BeforeJson: null,
                        AfterJson: null,
                        PayloadJson: null,
                        Metadata: metadata,
                        ErrorMessage: null,
                        StackTrace: null,
                        Details: null,
                        EntityName: pageName,
                        Description: $"{userName ?? "Un usuario"} visualizó {pageName}.",
                        HttpMethod: "GET",
                        RequestPath: route
                    );

                    var result = await dispatcher.DispatchAsync(
                        new RegisterAuditEventCommand(auditRequest),
                        cancellationToken
                    );

                    return EndpointResults.FromResult(result, httpContext);
                }
            )
            .WithTags("Audit Logs")
            .RequireAuthorization();

        return app;
    }

    private static Guid? ResolveUserId(ClaimsPrincipal user)
    {
        var candidates = new[]
        {
            ClaimTypes.NameIdentifier,
            "sub",
            "userId",
            "user_id",
            "uid",
        };

        foreach (var claimType in candidates)
        {
            var value = user.FindFirst(claimType)?.Value;
            if (Guid.TryParse(value, out var userId)) return userId;
        }

        return null;
    }

    private static string? ResolveUserName(ClaimsPrincipal user)
    {
        var candidates = new[]
        {
            ClaimTypes.Name,
            "name",
            "preferred_username",
            ClaimTypes.Email,
            "email",
        };

        foreach (var claimType in candidates)
        {
            var value = user.FindFirst(claimType)?.Value;
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        }

        return null;
    }

    private static string? ResolveIpAddress(HttpContext context)
    {
        var cloudflareIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(cloudflareIp)) return cloudflareIp.Trim();

        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp)) return realIp.Trim();

        return context.Connection.RemoteIpAddress?.ToString();
    }
}
