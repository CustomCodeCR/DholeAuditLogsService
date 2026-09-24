namespace Dhole.AuditLogs.Api.Authorization;

internal static class AuditLogsScopeNames
{
    public const string EventsView = "auditlogs.events.view";
    public const string EventsExport = "auditlogs.events.export";
    public const string EntityHistoryView = "auditlogs.entity-history.view";
    public const string UserHistoryView = "auditlogs.user-history.view";

    // Permite mostrar el historial de una cotización dentro de Pricing sin
    // conceder acceso al módulo completo de auditoría.
    public const string PricingRateView = "pricing.rate.view";
}
