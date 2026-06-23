using System.Text.Json;
using CustomCodeFramework.Cqrs.Dispatching;
using CustomCodeFramework.Messaging.Inbox;
using CustomCodeFramework.Redis.Streams.Abstractions;
using CustomCodeFramework.Redis.Streams.Messages;
using Dhole.AuditLogs.Application.AuditEvents.RegisterAuditEvent;
using Dhole.AuditLogs.Contracts.AuditEvents;
using Dhole.AuditLogs.Domain.Shared;
using Dhole.AuditLogs.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Dhole.AuditLogs.Worker.Streams;

internal sealed class AuditErrorStreamHandler(
    ServiceDbContext dbContext,
    ICommandDispatcher commandDispatcher,
    ILogger<AuditErrorStreamHandler> logger
) : IRedisStreamMessageHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
    };

    private const string ConsumerService = AuditLogsConstants.ServiceName;

    public string MessageType => AuditLogsConstants.MessageTypes.AuditErrorRegistered;

    public async Task HandleAsync(
        RedisStreamEnvelope envelope,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Recibido evento de error de auditoría {MessageType} con id Redis {MessageId}.",
            envelope.MessageType,
            envelope.MessageId
        );

        RegisterAuditEventRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<RegisterAuditEventRequest>(
                envelope.PayloadJson,
                JsonOptions
            );
        }
        catch (JsonException exception)
        {
            logger.LogWarning(
                exception,
                "El evento de error de auditoría {MessageId} no se pudo deserializar.",
                envelope.MessageId
            );

            return;
        }

        if (request is null)
        {
            logger.LogWarning(
                "El evento de error de auditoría {MessageId} llegó vacío o inválido.",
                envelope.MessageId
            );

            return;
        }

        if (request.EventId == Guid.Empty)
        {
            logger.LogWarning(
                "El evento de error de auditoría {MessageId} llegó sin EventId válido.",
                envelope.MessageId
            );

            return;
        }

        request = request with
        {
            Action = AuditLogsConstants.Actions.Error,
            EventType = request.EventType ?? AuditLogsConstants.MessageTypes.AuditErrorRegistered,
        };

        var alreadyProcessed = await dbContext.InboxMessages.AnyAsync(
            x => x.EventId == request.EventId && x.ConsumerService == ConsumerService,
            cancellationToken
        );

        if (alreadyProcessed)
        {
            logger.LogInformation(
                "El evento de error de auditoría {EventId} ya estaba registrado en inbox.",
                request.EventId
            );

            return;
        }

        await commandDispatcher.DispatchAsync(
            new RegisterAuditEventCommand(request),
            cancellationToken
        );

        var now = DateTime.UtcNow;

        var inboxMessage = new InboxMessage
        {
            EventId = request.EventId,
            EventType = request.EventType ?? envelope.MessageType,
            EventName = envelope.MessageType,
            SourceService = request.SourceService,
            ConsumerService = ConsumerService,
            CorrelationId =
                request.CorrelationId == Guid.Empty ? null : request.CorrelationId.ToString(),
            Status = InboxMessageStatus.Processed,
            ProcessedAtUtc = now,
            CreatedAtUtc = now,
        };

        await dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Evento de error de auditoría {EventId} de {SourceService} procesado y guardado en inbox.",
            request.EventId,
            request.SourceService
        );
    }
}
