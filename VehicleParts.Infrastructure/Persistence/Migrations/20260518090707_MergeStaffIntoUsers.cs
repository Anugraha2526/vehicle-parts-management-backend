using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleParts.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MergeStaffIntoUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. make PhoneNumber and Address nullable so staff rows can be inserted without these fields
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // 2. convert Role from text ("Customer","Admin","Staff") to integer (2,0,1)
            //    AlterColumn without USING would fail in PostgreSQL, so use raw SQL
            migrationBuilder.Sql(
                """
                ALTER TABLE "Users" ALTER COLUMN "Role" TYPE integer USING
                CASE "Role"
                    WHEN 'Admin'    THEN 0
                    WHEN 'Staff'    THEN 1
                    WHEN 'Customer' THEN 2
                    ELSE 2
                END;
                """);

            // 3. copy all staff records into Users with the SAME IDs so SalesInvoices.StaffId FKs remain valid
            migrationBuilder.Sql(
                """
                INSERT INTO "Users" ("Id", "FullName", "Email", "PhoneNumber", "Address", "PasswordHash", "Role", "IsActive", "CreatedAt", "CreatedAtUtc", "UpdatedAtUtc")
                SELECT
                    sm."Id",
                    sm."FullName",
                    sm."Email",
                    NULL,
                    NULL,
                    sm."PasswordHash",
                    sm."Role",
                    sm."IsActive",
                    sm."CreatedAtUtc",
                    sm."CreatedAtUtc",
                    sm."UpdatedAtUtc"
                FROM "StaffMembers" sm
                LEFT JOIN "Users" u ON u."Id" = sm."Id"
                WHERE u."Id" IS NULL;
                """);

            // 4. drop FK first, then drop the table (data already moved above)
            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_StaffMembers_StaffId",
                table: "SalesInvoices");

            migrationBuilder.DropTable(
                name: "StaffMembers");

            // 5. re-add FK now pointing to Users
            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_Users_StaffId",
                table: "SalesInvoices",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_Users_StaffId",
                table: "SalesInvoices");

            // restore Role column from integer back to text
            migrationBuilder.Sql(
                """
                ALTER TABLE "Users" ALTER COLUMN "Role" TYPE text USING
                CASE "Role"
                    WHEN 0 THEN 'Admin'
                    WHEN 1 THEN 'Staff'
                    WHEN 2 THEN 'Customer'
                    ELSE 'Customer'
                END;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "StaffMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_StaffMembers_StaffId",
                table: "SalesInvoices",
                column: "StaffId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
