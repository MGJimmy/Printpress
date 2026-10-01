using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260930233000_AddInventoryUsageSettlementVoid")]
    public class AddInventoryUsageSettlementVoid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                schema: "Inventory",
                table: "InventoryUsageSettlements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoidReason",
                schema: "Inventory",
                table: "InventoryUsageSettlements",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoidedAt",
                schema: "Inventory",
                table: "InventoryUsageSettlements",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoidedBy",
                schema: "Inventory",
                table: "InventoryUsageSettlements",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVoided",
                schema: "Inventory",
                table: "InventoryUsageSettlements");

            migrationBuilder.DropColumn(
                name: "VoidReason",
                schema: "Inventory",
                table: "InventoryUsageSettlements");

            migrationBuilder.DropColumn(
                name: "VoidedAt",
                schema: "Inventory",
                table: "InventoryUsageSettlements");

            migrationBuilder.DropColumn(
                name: "VoidedBy",
                schema: "Inventory",
                table: "InventoryUsageSettlements");
        }
    }
}
