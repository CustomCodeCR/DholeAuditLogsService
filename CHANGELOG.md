# Changelog

## [1.3.2] - 2026-11-08 (candidato F9, sin publicar)

- Sincroniza release global `rt-20261107-01` a `1.13.11` tras resolver los hallazgos de CI de Config, Reports y Gateway. Bump PATCH de DholeAuditLogsService `1.3.1` a `1.3.2`, sin modificar REST, eventos, precios ni datos; flags realtime OFF y PR F9 aún DRAFT.


## [1.3.1] - 2026-11-08 (candidato F9, sin publicación)

- Sincroniza metadatos de versión global del batch Dhole Realtime `rt-20261107-01` a `1.13.10`; actualización PATCH independiente de DholeAuditLogsService `1.3.0` → `1.3.1`. No cambia contratos REST, datos, lógica de negocio ni flags; requiere gates F0–F8 y CI de SHA final antes de merge.


## [1.3.0] - 2026-11-08 (candidato F6.06, SIN PUBLICAR)

- AuditLogs incorpora Outbox PostgreSQL adicional con migración EF aditiva y mensaje de invalidación mínima por cada evento de auditoría nuevo, en la misma transacción de persistencia.
- Worker separado reintenta envíos fallidos a `dhole.auditlogs.realtime.v1` usando EventId para idempotencia; nunca publica IP, usuario, stacktrace ni snapshots de auditoría.
- Pruebas de contrato y transaccionalidad sobre PostgreSQL real en CI, con flags de release OFF por defecto. Autoridad de lectura sigue en REST auditlogs.
- Componente 1.2.1 → 1.3.0, global candidato 1.13.0; sin merges ni despliegue.

## [1.2.1] - 2026-11-05

- Despliegues push master/develop sin cancelaciones ni aprobaciones manuales.
- Metadatos nativos y validación SemVer actualizados para Dhole.

## [1.2.0] - 2026-11-05

- Incorporates universal Dhole SemVer policy with GitHub Actions checks.
- Synchronizes service assembly metadata with the ecosystem version.
- Adds permanent Codex instructions and coordinated release requirements.
