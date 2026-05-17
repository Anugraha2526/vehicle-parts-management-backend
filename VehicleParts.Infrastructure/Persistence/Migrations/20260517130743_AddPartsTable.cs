using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleParts.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPartsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "OemCode",
                table: "Parts");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Parts",
                newName: "UnitCost");

            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "Parts",
                newName: "QuantityInStock");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Parts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Parts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartName",
                table: "Parts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "Parts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SellingPrice",
                table: "Parts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "Parts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_CustomerId",
                table: "SalesInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartNumber",
                table: "Parts",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parts_VendorId",
                table: "Parts",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Vendors_VendorId",
                table: "Parts",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_Customer_CustomerId",
                table: "SalesInvoices",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Vendors_VendorId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_Customer_CustomerId",
                table: "SalesInvoices");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_SalesInvoices_CustomerId",
                table: "SalesInvoices");

            migrationBuilder.DropIndex(
                name: "IX_Parts_PartNumber",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_Parts_VendorId",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "PartName",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Parts");

            migrationBuilder.RenameColumn(
                name: "UnitCost",
                table: "Parts",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "QuantityInStock",
                table: "Parts",
                newName: "StockQuantity");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Parts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OemCode",
                table: "Parts",
                type: "text",
                nullable: true);
        }
    }
}
