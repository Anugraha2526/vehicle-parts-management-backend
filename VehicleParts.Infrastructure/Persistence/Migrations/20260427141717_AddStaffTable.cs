using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleParts.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StaffMembers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "StaffMembers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "StaffMembers");
        }
    }
}
