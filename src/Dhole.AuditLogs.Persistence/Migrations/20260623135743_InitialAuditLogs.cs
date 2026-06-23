using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dhole.AuditLogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auditlogs");

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                schema: "auditlogs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_service = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    event_type = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ip_address = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    before_json = table.Column<string>(type: "jsonb", nullable: true),
                    after_json = table.Column<string>(type: "jsonb", nullable: true),
                    payload_json = table.Column<string>(type: "jsonb", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    error_message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    stack_trace = table.Column<string>(type: "text", nullable: true),
                    Details = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_audit_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "auditlogs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    event_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    source_service = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    consumer_service = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_inbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_action",
                schema: "auditlogs",
                table: "AuditEvents",
                column: "action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_correlation_id",
                schema: "auditlogs",
                table: "AuditEvents",
                column: "correlation_id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_entity_type_entity_id",
                schema: "auditlogs",
                table: "AuditEvents",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_event_id",
                schema: "auditlogs",
                table: "AuditEvents",
                column: "event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_event_type",
                schema: "auditlogs",
                table: "AuditEvents",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_occurred_at",
                schema: "auditlogs",
                table: "AuditEvents",
                column: "occurred_at");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_source_service_occurred_at",
                schema: "auditlogs",
                table: "AuditEvents",
                columns: new[] { "source_service", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_user_id_occurred_at",
                schema: "auditlogs",
                table: "AuditEvents",
                columns: new[] { "user_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_inbox_messages_event_id_consumer_service",
                schema: "auditlogs",
                table: "inbox_messages",
                columns: new[] { "event_id", "consumer_service" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inbox_messages_status_created_at",
                schema: "auditlogs",
                table: "inbox_messages",
                columns: new[] { "status", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEvents",
                schema: "auditlogs");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "auditlogs");
        }
    }
}
