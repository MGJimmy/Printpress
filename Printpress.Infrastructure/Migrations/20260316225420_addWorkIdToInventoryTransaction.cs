using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addWorkIdToInventoryTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkerId",
                schema: "Inventory",
                table: "InventoryTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_WorkerId",
                schema: "Inventory",
                table: "InventoryTransactions",
                column: "WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Workers_WorkerId",
                schema: "Inventory",
                table: "InventoryTransactions",
                column: "WorkerId",
                principalSchema: "HR",
                principalTable: "Workers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Workers_WorkerId",
                schema: "Inventory",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_WorkerId",
                schema: "Inventory",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                schema: "Inventory",
                table: "InventoryTransactions");
        }
    }
}
