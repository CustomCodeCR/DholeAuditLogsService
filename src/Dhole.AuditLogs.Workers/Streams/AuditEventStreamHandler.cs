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

internal sealed class AuditEventStreamHandler(
    ServiceDbContext dbContext,
    ICommandDispatcher commandDispatcher,
    ILogger<AuditEventStreamHandler> logger
) : IRedisStreamMessageHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
    };

    private const string ConsumerService = AuditLogsConstants.ServiceName;

    public string MessageType => "audit.event.registered";

    public async Task HandleAsync(
        RedisStreamEnvelope envelope,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Recibido evento de auditoría {MessageType} con id {MessageId}.",
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
                "El evento de auditoría {MessageId} no se pudo deserializar.",
                envelope.MessageId
            );

            return;
        }

        if (request is null)
        {
            logger.LogWarning(
                "El evento de auditoría {MessageId} llegó vacío o inválido.",
                envelope.MessageId
            );

            return;
        }

        var alreadyProcessed = await dbContext.InboxMessages.AnyAsync(
            x => x.EventId == request.EventId && x.ConsumerService == ConsumerService,
            cancellationToken
        );

        if (alreadyProcessed)
        {
            logger.LogInformation(
                "El evento de auditoría {EventId} ya fue procesado.",
                request.EventId
            );

            return;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
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
            CorrelationId = request.CorrelationId.ToString(),
            Status = InboxMessageStatus.Processed,
            ProcessedAtUtc = now,
            CreatedAtUtc = now,
        };

        await dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);

        await commandDispatcher.DispatchAsync(
            new RegisterAuditEventCommand(request),
            cancellationToken
        );

        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Evento de auditoría {EventId} de {SourceService} procesado correctamente.",
            request.EventId,
            request.SourceService
        );
    }
}
