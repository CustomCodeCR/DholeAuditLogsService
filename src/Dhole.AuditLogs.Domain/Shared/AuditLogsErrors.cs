using CustomCodeFramework.Core.Results;

namespace Dhole.AuditLogs.Domain.Shared;

public static class AuditLogsErrors
{
    public static readonly Error NotFound = new(
        "AuditLogs.NotFound",
        "Evento de auditoría no encontrado."
    );

    public static readonly Error AuditEventNotFound = new(
        "AuditLogs.AuditEventNotFound",
        "Evento de auditoría no encontrado."
    );

    public static readonly Error InvalidSourceService = new(
        "AuditLogs.InvalidSourceService",
        "El servicio origen es requerido."
    );

    public static readonly Error InvalidAction = new(
        "AuditLogs.InvalidAction",
        "La acción de auditoría es requerida."
    );

    public static readonly Error InvalidCorrelationId = new(
        "AuditLogs.InvalidCorrelationId",
        "El identificador de correlación es requerido."
    );

    public static readonly Error InvalidEventId = new(
        "AuditLogs.InvalidEventId",
        "El identificador del evento es requerido."
    );

    public static readonly Error InvalidDateRange = new(
        "AuditLogs.InvalidDateRange",
        "El rango de fechas no es válido."
    );

    public static readonly Error InvalidEntity = new(
        "AuditLogs.InvalidEntity",
        "La entidad consultada no es válida."
    );

    public static readonly Error InvalidUser = new(
        "AuditLogs.InvalidUser",
        "El usuario consultado no es válido."
    );

    public static readonly Error InvalidExportRequest = new(
        "AuditLogs.InvalidExportRequest",
        "La solicitud de exportación no es válida."
    );
}
