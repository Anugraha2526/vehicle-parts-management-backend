using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vehicle_parts_management_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPartRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "PartRequests");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PartRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedAtUtc",
                table: "PartRequests",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "PartName",
                table: "PartRequests",
                newName: "RequestedPartName");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PartRequests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "PartRequests",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "PartRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "PartRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedPartName",
                table: "PartRequests",
                newName: "PartName");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "PartRequests",
                newName: "RequestedAtUtc");

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "PartRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PartRequests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
