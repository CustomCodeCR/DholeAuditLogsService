using System.Text.Json;
using CustomCodeFramework.Core.Domain.Entities;
using Dhole.AuditLogs.Domain.AuditEvents.ValueObjects;

namespace Dhole.AuditLogs.Domain.AuditEvents.Entities;

public sealed class AuditEvent : AggregateRoot<Guid>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };

    private AuditEvent() { }

    private AuditEvent(
        Guid id,
        Guid eventId,
        Guid correlationId,
        string sourceService,
        string? entityType,
        Guid? entityId,
        string action,
        string? eventType,
        Guid? userId,
        string? userName,
        string? ipAddress,
        string? userAgent,
        DateTime occurredAt,
        string? beforeJson,
        string? afterJson,
        string? payloadJson,
        string? metadataJson,
        string? errorMessage,
        string? stackTrace,
        string? detailsJson
    )
        : base(id)
    {
        EventId = eventId;
        CorrelationId = correlationId;
        SourceService = sourceService;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        EventType = eventType;
        UserId = userId;
        UserName = userName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        OccurredAt = occurredAt;
        BeforeJson = beforeJson;
        AfterJson = afterJson;
        PayloadJson = payloadJson;
        MetadataJson = metadataJson;
        ErrorMessage = errorMessage;
        StackTrace = stackTrace;
        DetailsJson = detailsJson;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid EventId { get; private set; }

    public Guid CorrelationId { get; private set; }

    public string SourceService { get; private set; } = default!;

    public string? EntityType { get; private set; }

    public Guid? EntityId { get; private set; }

    public string Action { get; private set; } = default!;

    public string? EventType { get; private set; }

    public Guid? UserId { get; private set; }

    public string? UserName { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public DateTime OccurredAt { get; private set; }

    public string? BeforeJson { get; private set; }

    public string? AfterJson { get; private set; }

    public string? PayloadJson { get; private set; }

    public string? MetadataJson { get; private set; }

    public string? ErrorMessage { get; private set; }

    public string? StackTrace { get; private set; }

    public string? DetailsJson { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<AuditEventDetailJson> Details => GetDetails();

    public static AuditEvent Create(
        Guid eventId,
        Guid correlationId,
        string sourceService,
        string? entityType,
        Guid? entityId,
        string action,
        string? eventType,
        Guid? userId,
        string? userName,
        string? ipAddress,
        string? userAgent,
        DateTime occurredAt,
        string? beforeJson = null,
        string? afterJson = null,
        string? payloadJson = null,
        string? metadataJson = null,
        string? errorMessage = null,
        string? stackTrace = null,
        IReadOnlyCollection<AuditEventDetailJson>? details = null
    )
    {
        return new AuditEvent(
            Guid.NewGuid(),
            eventId,
            correlationId,
            sourceService.Trim(),
            TrimOrNull(entityType),
            entityId,
            action.Trim(),
            TrimOrNull(eventType),
            userId,
            TrimOrNull(userName),
            TrimOrNull(ipAddress),
            TrimOrNull(userAgent),
            occurredAt,
            NormalizeJson(beforeJson),
            NormalizeJson(afterJson),
            NormalizeJson(payloadJson),
            NormalizeJson(metadataJson),
            TrimOrNull(errorMessage),
            stackTrace,
            SerializeDetails(details)
        );
    }

    public void AddDetail(string? fieldName, string? oldValue, string? newValue, string? metadata)
    {
        var details = GetDetails().ToList();

        details.Add(
            new AuditEventDetailJson(
                TrimOrNull(fieldName),
                oldValue,
                newValue,
                NormalizeJson(metadata)
            )
        );

        DetailsJson = SerializeDetails(details);
    }

    private IReadOnlyCollection<AuditEventDetailJson> GetDetails()
    {
        if (string.IsNullOrWhiteSpace(DetailsJson))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<IReadOnlyCollection<AuditEventDetailJson>>(
                    DetailsJson,
                    JsonOptions
                ) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static string? SerializeDetails(IReadOnlyCollection<AuditEventDetailJson>? details)
    {
        if (details is null || details.Count == 0)
        {
            return null;
        }

        return JsonSerializer.Serialize(details, JsonOptions);
    }

    private static string? TrimOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    private static string? NormalizeJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();

        try
        {
            using var document = JsonDocument.Parse(trimmed);
            return document.RootElement.GetRawText();
        }
        catch
        {
            return JsonSerializer.Serialize(trimmed, JsonOptions);
        }
    }
}
