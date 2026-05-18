using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleParts.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PartsSchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_Customer_CustomerId",
                table: "SalesInvoices");

            migrationBuilder.DropTable(
                name: "Customer");

            // Backfill legacy references before adding new foreign keys.
            migrationBuilder.Sql(
                """
                INSERT INTO "Users" ("Id", "FullName", "Email", "PhoneNumber", "Address", "PasswordHash", "Role", "IsActive", "CreatedAt", "CreatedAtUtc", "UpdatedAtUtc")
                SELECT DISTINCT
                    s."CustomerId",
                    'Legacy Customer ' || SUBSTRING(s."CustomerId"::text, 1, 8),
                    'legacy-customer-' || SUBSTRING(s."CustomerId"::text, 1, 8) || '@local.invalid',
                    '0000000000',
                    'Migrated from legacy SalesInvoices reference',
                    '',
                    'Customer',
                    TRUE,
                    NOW(),
                    NOW(),
                    NOW()
                FROM "SalesInvoices" s
                LEFT JOIN "Users" u ON u."Id" = s."CustomerId"
                WHERE u."Id" IS NULL;
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO "StaffMembers" ("Id", "FullName", "Email", "PasswordHash", "Role", "IsActive", "CreatedAtUtc", "UpdatedAtUtc")
                SELECT DISTINCT
                    s."StaffId",
                    'Legacy Staff ' || SUBSTRING(s."StaffId"::text, 1, 8),
                    'legacy-staff-' || SUBSTRING(s."StaffId"::text, 1, 8) || '@local.invalid',
                    '',
                    1,
                    TRUE,
                    NOW(),
                    NOW()
                FROM "SalesInvoices" s
                LEFT JOIN "StaffMembers" st ON st."Id" = s."StaffId"
                WHERE st."Id" IS NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_StaffId",
                table: "SalesInvoices",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_StaffMembers_StaffId",
                table: "SalesInvoices",
                column: "StaffId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_Users_CustomerId",
                table: "SalesInvoices",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_StaffMembers_StaffId",
                table: "SalesInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_Users_CustomerId",
                table: "SalesInvoices");

            migrationBuilder.DropIndex(
                name: "IX_SalesInvoices_StaffId",
                table: "SalesInvoices");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_Customer_CustomerId",
                table: "SalesInvoices",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
