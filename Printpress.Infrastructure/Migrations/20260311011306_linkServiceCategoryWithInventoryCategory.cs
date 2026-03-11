using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class linkServiceCategoryWithInventoryCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryItemCategoryId",
                schema: "Orders",
                table: "ServiceCategorys",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategorys_InventoryItemCategoryId",
                schema: "Orders",
                table: "ServiceCategorys",
                column: "InventoryItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCategorys_InventoryItemCategory_LKPs_InventoryItemCa~",
                schema: "Orders",
                table: "ServiceCategorys",
                column: "InventoryItemCategoryId",
                principalSchema: "Inventory",
                principalTable: "InventoryItemCategory_LKPs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCategorys_InventoryItemCategory_LKPs_InventoryItemCa~",
                schema: "Orders",
                table: "ServiceCategorys");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCategorys_InventoryItemCategoryId",
                schema: "Orders",
                table: "ServiceCategorys");

            migrationBuilder.DropColumn(
                name: "InventoryItemCategoryId",
                schema: "Orders",
                table: "ServiceCategorys");
        }
    }
}
