using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dhole.AuditLogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DetailedAuditContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "auditlogs",
                table: "AuditEvents",
                type: "character varying(1500)",
                maxLength: 1500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "entity_name",
                schema: "auditlogs",
                table: "AuditEvents",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "http_method",
                schema: "auditlogs",
                table: "AuditEvents",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "request_path",
                schema: "auditlogs",
                table: "AuditEvents",
                type: "character varying(800)",
                maxLength: 800,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_entity_name",
                schema: "auditlogs",
                table: "AuditEvents",
                column: "entity_name");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_ip_address",
                schema: "auditlogs",
                table: "AuditEvents",
                column: "ip_address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditEvents_entity_name",
                schema: "auditlogs",
                table: "AuditEvents");

            migrationBuilder.DropIndex(
                name: "IX_AuditEvents_ip_address",
                schema: "auditlogs",
                table: "AuditEvents");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "auditlogs",
                table: "AuditEvents");

            migrationBuilder.DropColumn(
                name: "entity_name",
                schema: "auditlogs",
                table: "AuditEvents");

            migrationBuilder.DropColumn(
                name: "http_method",
                schema: "auditlogs",
                table: "AuditEvents");

            migrationBuilder.DropColumn(
                name: "request_path",
                schema: "auditlogs",
                table: "AuditEvents");
        }
    }
}
