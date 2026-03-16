using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deactivateServiceInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Orders",
                table: "Services",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Inventory",
                table: "InventoryItems",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Orders",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Inventory",
                table: "InventoryItems");
        }
    }
}
