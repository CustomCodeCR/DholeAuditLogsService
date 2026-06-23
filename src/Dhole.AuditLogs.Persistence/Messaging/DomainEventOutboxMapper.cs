using CustomCodeFramework.Core.Domain.Events;

namespace Dhole.AuditLogs.Persistence.Messaging;

internal static class DomainEventOutboxMapper
{
    public static string GetEventName(IDomainEvent domainEvent)
    {
        return $"auditlogs.{domainEvent.GetType().Name}";
    }

    public static string GetEventType(IDomainEvent domainEvent)
    {
        return domainEvent.GetType().FullName ?? domainEvent.GetType().Name;
    }
}
