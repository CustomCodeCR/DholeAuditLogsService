using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Dhole.AuditLogs.Persistence.Messaging;

/// <summary>
/// F6.06: one safe invalidation per persisted audit record, not the original
/// event payload, actor, IP, token, before/after snapshots or stack trace.
/// </summary>
public static class AuditRealtimeInvalidation
{
    public const string ReleaseId = "rt-20261107-01";
    public const string Type = "audit.event.created.v1";
    public const string Stream = "dhole.auditlogs.realtime.v1";
    private static long _lastVersionHint;

    public static bool IsEnabled(IConfiguration? config) =>
        config?.GetValue<bool>("Features:UniversalRealtimeEnabled") == true &&
        config.GetValue<bool>("Realtime:Outbox:Enabled") &&
        config.GetValue<bool>("Realtime:BatchApproved") &&
        string.Equals(config["Realtime:ReleaseId"], ReleaseId, StringComparison.Ordinal);

    public static long NextVersionHint()
    {
        while (true)
        {
            var microsecond = (DateTimeOffset.UtcNow.Ticks - DateTimeOffset.UnixEpoch.Ticks) / 10;
            var prior = Volatile.Read(ref _lastVersionHint);
            var next = Math.Max(microsecond, prior + 1);
            if (Interlocked.CompareExchange(ref _lastVersionHint, next, prior) == prior)
                return next;
        }
    }

    public static string Serialize(Guid eventId, Guid auditId, long version,
        string correlationId, DateTimeOffset timestamp)
    {
        if (eventId == Guid.Empty || auditId == Guid.Empty || version <= 0 ||
            string.IsNullOrWhiteSpace(correlationId) || timestamp.Offset != TimeSpan.Zero)
            throw new ArgumentException("Invalid audit realtime notification.");
        return JsonSerializer.Serialize(new
        {
            eventId,
            eventType = Type,
            schemaVersion = 1,
            aggregateType = "event",
            aggregateId = auditId.ToString("D"),
            aggregateVersion = version,
            occurredAtUtc = timestamp,
            producer = "DholeAuditLogsService",
            correlationId,
            audienceRef = $"audit:event:{auditId:D}",
            payload = new { changedFields = new[] { "event" } }
        });
    }
}
