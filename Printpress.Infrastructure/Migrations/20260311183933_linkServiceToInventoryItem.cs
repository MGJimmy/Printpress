using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class linkServiceToInventoryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                schema: "Orders",
                table: "Services",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_InventoryItemId",
                schema: "Orders",
                table: "Services",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_InventoryItems_InventoryItemId",
                schema: "Orders",
                table: "Services",
                column: "InventoryItemId",
                principalSchema: "Inventory",
                principalTable: "InventoryItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_InventoryItems_InventoryItemId",
                schema: "Orders",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_InventoryItemId",
                schema: "Orders",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                schema: "Orders",
                table: "Services");
        }
    }
}
