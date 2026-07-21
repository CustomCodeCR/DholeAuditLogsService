namespace Dhole.AuditLogs.Domain.Shared;

public static class AuditLogsConstants
{
    public const string ServiceName = "AuditLogs";

    public static class Streams
    {
        public const string AuditEvents = "dhole.audit.events";
        public const string AuditEventsConsumerGroup = "dhole-auditlogs";
    }

    public static class MessageTypes
    {
        public const string AuditEventRegistered = "audit.event.registered";
        public const string AuditErrorRegistered = "audit.error.registered";
    }

    public static class Scopes
    {
        public const string EventsView = "auditlogs.events.view";
        public const string EventsExport = "auditlogs.events.export";
        public const string EntityHistoryView = "auditlogs.entity-history.view";
        public const string UserHistoryView = "auditlogs.user-history.view";
    }

    public static class Actions
    {
        public const string Created = "created";
        public const string Updated = "updated";
        public const string Deleted = "deleted";
        public const string Activated = "activated";
        public const string Inactivated = "inactivated";
        public const string Blocked = "blocked";
        public const string Unblocked = "unblocked";
        public const string Approved = "approved";
        public const string Rejected = "rejected";
        public const string AccessDenied = "access_denied";
        public const string PermissionChanged = "permission_changed";
        public const string SessionRevoked = "session_revoked";
        public const string Login = "login";
        public const string Logout = "logout";
        public const string Error = "error";
        public const string Viewed = "viewed";
        public const string Exported = "exported";
        public const string Chat = "chat";
        public const string Analyzed = "analyzed";
    }

    public static class Results
    {
        public const string Success = "success";
        public const string Failed = "failed";
        public const string Denied = "denied";
    }

    public static class EntityTypes
    {
        public const string AuditEvent = "AuditEvent";
        public const string AiChat = "AiChat";
        public const string PricingAiAnalysis = "PricingAiAnalysis";
    }
}
