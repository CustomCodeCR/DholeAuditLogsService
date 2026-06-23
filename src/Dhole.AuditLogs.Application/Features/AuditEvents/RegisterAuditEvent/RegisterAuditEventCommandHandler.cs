using CustomCodeFramework.Core.Results;
using CustomCodeFramework.Cqrs.Commands;
using CustomCodeFramework.Persistence.Abstractions;
using Dhole.AuditLogs.Application.Abstractions.Mongo;
using Dhole.AuditLogs.Application.Abstractions.Repositories;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;
using Dhole.AuditLogs.Domain.AuditEvents.ValueObjects;
using Dhole.AuditLogs.Domain.Shared;
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
        {
            return Result.Failure<Guid>(AuditLogsErrors.InvalidEventId);
        }

        if (request.CorrelationId == Guid.Empty)
        {
            return Result.Failure<Guid>(AuditLogsErrors.InvalidCorrelationId);
        }

        if (string.IsNullOrWhiteSpace(request.SourceService))
        {
            return Result.Failure<Guid>(AuditLogsErrors.InvalidSourceService);
        }

        if (string.IsNullOrWhiteSpace(request.Action))
        {
            return Result.Failure<Guid>(AuditLogsErrors.InvalidAction);
        }

        var existingAuditEvent = await auditEvents.GetByEventIdAsync(
            request.EventId,
            cancellationToken
        );

        if (existingAuditEvent is not null)
        {
            return Result.Success(existingAuditEvent.Id);
        }

        var details = request
            .Details?.Select(x => new AuditEventDetailJson(
                x.FieldName,
                x.OldValue,
                x.NewValue,
                x.Metadata
            ))
            .ToList();

        var occurredAt = request.OccurredAt == default ? DateTime.UtcNow : request.OccurredAt;

        var auditEvent = AuditEvent.Create(
            eventId: request.EventId,
            correlationId: request.CorrelationId,
            sourceService: request.SourceService,
            entityType: request.EntityType,
            entityId: request.EntityId,
            action: request.Action,
            eventType: request.EventType,
            userId: request.UserId,
            userName: request.UserName,
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            occurredAt: occurredAt,
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
            auditEvent.Action,
            auditEvent.EventType,
            auditEvent.UserId,
            auditEvent.UserName,
            auditEvent.IpAddress,
            auditEvent.UserAgent,
            auditEvent.OccurredAt,
            auditEvent.CreatedAt,
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
}
