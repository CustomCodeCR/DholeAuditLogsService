using System.Text.Json;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;
using Dhole.AuditLogs.Persistence.DbContexts;
using Dhole.AuditLogs.Persistence.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dhole.AuditLogs.IntegrationTests;

[TestClass]
public sealed class F6AuditRealtimeTests
{
    private static IConfiguration Flags(bool approved = true) =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Features:UniversalRealtimeEnabled"] = "true",
            ["Realtime:Outbox:Enabled"] = "true",
            ["Realtime:BatchApproved"] = approved.ToString(),
            ["Realtime:ReleaseId"] = AuditRealtimeInvalidation.ReleaseId
        }).Build();

    [TestMethod]
    public void Only_approved_release_emits_audit_invalidations()
    {
        Assert.IsFalse(AuditRealtimeInvalidation.IsEnabled(null));
        Assert.IsFalse(AuditRealtimeInvalidation.IsEnabled(Flags(false)));
        var enabled = Flags();
        Assert.IsTrue(AuditRealtimeInvalidation.IsEnabled(enabled));
        enabled["Realtime:ReleaseId"] = "unrelated";
        Assert.IsFalse(AuditRealtimeInvalidation.IsEnabled(enabled));
    }

    [TestMethod]
    public void Audit_payload_never_includes_sensitive_audit_record()
    {
        var eventId = Guid.NewGuid();
        var auditId = Guid.NewGuid();
        var json = AuditRealtimeInvalidation.Serialize(eventId, auditId,
            AuditRealtimeInvalidation.NextVersionHint(), "test",
            DateTimeOffset.UtcNow);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        Assert.AreEqual(AuditRealtimeInvalidation.Type, root.GetProperty("eventType").GetString());
        Assert.AreEqual($"audit:event:{auditId:D}", root.GetProperty("audienceRef").GetString());
        CollectionAssert.AreEquivalent(new[] { "changedFields" },
            root.GetProperty("payload").EnumerateObject().Select(p => p.Name).ToArray());
        foreach (var forbidden in new[] { "userName", "ipAddress", "beforeJson", "afterJson", "stackTrace", "token" })
            Assert.IsFalse(json.Contains(forbidden, StringComparison.OrdinalIgnoreCase), forbidden);
    }

    [TestMethod]
    public async Task Real_PostgreSQL_audit_insert_and_Outbox_commit_together()
    {
        var connectionString = Environment.GetEnvironmentVariable("AUDIT_F6_POSTGRES")
            ?? throw new InvalidOperationException("AUDIT_F6_POSTGRES CI database required");
        var options = new DbContextOptionsBuilder<ServiceDbContext>()
            .UseNpgsql(connectionString).Options;
        await using var db = new ServiceDbContext(options, Flags());
        await db.Database.MigrateAsync();
        var audit = AuditEvent.Create(
            Guid.NewGuid(), Guid.NewGuid(), "Dhole.F6.CI", null, null, null,
            "insert", null, null, null, null, null, DateTime.UtcNow);
        db.AuditEvents.Add(audit);
        await db.SaveChangesAsync();
        var emitted = await db.OutboxMessages.SingleAsync(x =>
            x.EventName == AuditRealtimeInvalidation.Type &&
            x.PayloadJson.Contains(audit.Id.ToString("D")));
        using var json = JsonDocument.Parse(emitted.PayloadJson);
        Assert.AreEqual(audit.Id, json.RootElement.GetProperty("aggregateId").GetGuid());
        Assert.AreEqual(1, await db.AuditEvents.CountAsync(x => x.Id == audit.Id));
        Assert.AreEqual(1, await db.OutboxMessages.CountAsync(x => x.EventId == emitted.EventId));

        await using var off = new ServiceDbContext(options, Flags(false));
        var second = AuditEvent.Create(
            Guid.NewGuid(), Guid.NewGuid(), "Dhole.F6.CI", null, null, null,
            "insert", null, null, null, null, null, DateTime.UtcNow);
        off.AuditEvents.Add(second);
        await off.SaveChangesAsync();
        Assert.AreEqual(0, await off.OutboxMessages.CountAsync(x =>
            x.PayloadJson.Contains(second.Id.ToString("D")) &&
            x.EventName == AuditRealtimeInvalidation.Type));
    }
}
