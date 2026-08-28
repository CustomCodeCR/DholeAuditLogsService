using System.Text.Json;
using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Commands;
using CustomCodeFramework.Persistence.Abstractions;
using Dhole.AuditLogs.Application.Abstractions.Mongo;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;
using Dhole.AuditLogs.Domain.AuditEvents.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Dhole.AuditLogs.Application.AuditEvents.RegisterAuditEvent;

public sealed class RegisterAuditEventCommandHandler(
    IAuditEventRepository auditEvents,
    IAuditEventPayloadWriter payloadWriter,
    IAuditErrorDetailWriter errorDetailWriter,
    IUnitOfWork unitOfWork,
    ILogger<RegisterAuditEventCommandHandler> logger
) : ICommandHandler<RegisterAuditEventCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(
        RegisterAuditEventCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var request = command.Request;

        if (request.EventId == Guid.Empty)
            return Result.Failure<Guid>(Domain.Shared.AuditLogsErrors.InvalidEventId);
        if (request.CorrelationId == Guid.Empty)
            return Result.Failure<Guid>(Domain.Shared.AuditLogsErrors.InvalidCorrelationId);
        if (string.IsNullOrWhiteSpace(request.SourceService))
            return Result.Failure<Guid>(Domain.Shared.AuditLogsErrors.InvalidSourceService);
        if (string.IsNullOrWhiteSpace(request.Action))
            return Result.Failure<Guid>(Domain.Shared.AuditLogsErrors.InvalidAction);

        var existingAuditEvent = await auditEvents.GetByEventIdAsync(request.EventId, cancellationToken);
        if (existingAuditEvent is not null)
            return Result.Success(existingAuditEvent.Id);

        var details = request.Details?.Select(x => new AuditEventDetailJson(
            x.FieldName,
            x.OldValue,
            x.NewValue,
            x.Metadata
        )).ToList();

        var occurredAt = request.OccurredAt == default ? DateTime.UtcNow : request.OccurredAt;
        var entityName = FirstNotBlank(
            request.EntityName,
            TryExtract(request.Metadata, "entityName", "name", "displayName", "rateName", "title", "code", "email", "fileName"),
            TryExtract(request.AfterJson, "name", "displayName", "rateName", "title", "code", "email", "fileName"),
            TryExtract(request.PayloadJson, "name", "displayName", "rateName", "title", "code", "email", "fileName")
        );
        var requestPath = FirstNotBlank(
            request.RequestPath,
            TryExtract(request.Metadata, "requestPath", "path", "route", "url")
        );
        var httpMethod = FirstNotBlank(
            request.HttpMethod,
            TryExtract(request.Metadata, "httpMethod", "method")
        );
        var description = FirstNotBlank(
            request.Description,
            BuildDescription(request.Action, request.EntityType, entityName, request.EntityId, request.UserName)
        );

        var auditEvent = AuditEvent.Create(
            eventId: request.EventId,
            correlationId: request.CorrelationId,
            sourceService: request.SourceService,
            entityType: request.EntityType,
            entityId: request.EntityId,
            entityName: entityName,
            action: request.Action,
            eventType: request.EventType,
            userId: request.UserId,
            userName: request.UserName,
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            occurredAt: occurredAt,
            description: description,
            httpMethod: httpMethod,
            requestPath: requestPath,
            beforeJson: request.BeforeJson,
            afterJson: request.AfterJson,
            payloadJson: request.PayloadJson,
            metadataJson: request.Metadata,
            errorMessage: request.ErrorMessage,
            stackTrace: request.StackTrace,
            details: details
        );

        await auditEvents.AddAsync(auditEvent, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var auditEventDto = new AuditEventDto(
            auditEvent.Id,
            auditEvent.EventId,
            auditEvent.CorrelationId,
            auditEvent.SourceService,
            auditEvent.EntityType,
            auditEvent.EntityId,
            auditEvent.EntityName,
            auditEvent.Action,
            auditEvent.EventType,
            auditEvent.UserId,
            auditEvent.UserName,
            auditEvent.IpAddress,
            auditEvent.UserAgent,
            auditEvent.OccurredAt,
            auditEvent.CreatedAt,
            auditEvent.Description,
            auditEvent.HttpMethod,
            auditEvent.RequestPath,
            auditEvent.BeforeJson,
            auditEvent.AfterJson,
            auditEvent.PayloadJson,
            auditEvent.MetadataJson,
            auditEvent.ErrorMessage,
            auditEvent.StackTrace,
            auditEvent.DetailsJson
        );

        try
        {
            await payloadWriter.WriteAsync(auditEventDto, cancellationToken);
            await errorDetailWriter.WriteAsync(auditEventDto, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "No se pudo escribir el snapshot Mongo del evento de auditoría {EventId}. El evento principal sí quedó guardado en PostgreSQL.",
                request.EventId
            );
        }

        return Result.Success(auditEvent.Id);
    }

    private static string? FirstNotBlank(params string?[] values)
        => values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim();

    private static string? TryExtract(string? json, params string[] keys)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            using var document = JsonDocument.Parse(json);
            return FindValue(document.RootElement, keys);
        }
        catch
        {
            return null;
        }
    }

    private static string? FindValue(JsonElement element, IReadOnlyCollection<string> keys)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (keys.Any(key => string.Equals(key, property.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return property.Value.ValueKind switch
                    {
                        JsonValueKind.String => property.Value.GetString(),
                        JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => property.Value.ToString(),
                        _ => null,
                    };
                }

                var nested = FindValue(property.Value, keys);
                if (!string.IsNullOrWhiteSpace(nested)) return nested;
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var child in element.EnumerateArray())
            {
                var nested = FindValue(child, keys);
                if (!string.IsNullOrWhiteSpace(nested)) return nested;
            }
        }

        return null;
    }

    private static string BuildDescription(
        string action,
        string? entityType,
        string? entityName,
        Guid? entityId,
        string? userName
    )
    {
        var actor = string.IsNullOrWhiteSpace(userName) ? "Un usuario" : userName.Trim();
        var target = !string.IsNullOrWhiteSpace(entityName)
            ? entityName.Trim()
            : !string.IsNullOrWhiteSpace(entityType)
                ? entityId.HasValue ? $"{entityType} {entityId}" : entityType.Trim()
                : "el sistema";

        var verb = action.Trim().ToLowerInvariant() switch
        {
            "created" => "creó",
            "updated" => "modificó",
            "deleted" => "eliminó",
            "viewed" => "visualizó",
            "approved" => "aprobó",
            "rejected" => "rechazó",
            "activated" => "activó",
            "inactivated" => "inactivó",
            "blocked" => "bloqueó",
            "unblocked" => "desbloqueó",
            "login" => "inició sesión en",
            "logout" => "cerró sesión en",
            "exported" => "exportó",
            "access_denied" => "intentó acceder sin permiso a",
            "permission_changed" => "cambió permisos de",
            "session_revoked" => "revocó una sesión de",
            "analyzed" => "analizó",
            "chat" => "interactuó con",
            "error" => "generó un error en",
            _ => action.Trim(),
        };

        return $"{actor} {verb} {target}.";
    }
}
