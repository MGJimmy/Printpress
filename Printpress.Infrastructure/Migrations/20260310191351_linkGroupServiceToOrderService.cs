using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class linkGroupServiceToOrderService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupService_OrderGroups_OrderGroupId",
                schema: "Orders",
                table: "OrderGroupService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupService_Services_ServiceId",
                schema: "Orders",
                table: "OrderGroupService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderGroupService",
                schema: "Orders",
                table: "OrderGroupService");

            migrationBuilder.RenameTable(
                name: "OrderGroupService",
                schema: "Orders",
                newName: "OrderGroupServices",
                newSchema: "Orders");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "Orders",
                table: "OrderGroupServices",
                newName: "OrderServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderGroupService_ServiceId",
                schema: "Orders",
                table: "OrderGroupServices",
                newName: "IX_OrderGroupServices_OrderServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderGroupService_OrderGroupId",
                schema: "Orders",
                table: "OrderGroupServices",
                newName: "IX_OrderGroupServices_OrderGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderGroupServices",
                schema: "Orders",
                table: "OrderGroupServices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroupServices_OrderGroups_OrderGroupId",
                schema: "Orders",
                table: "OrderGroupServices",
                column: "OrderGroupId",
                principalSchema: "Orders",
                principalTable: "OrderGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroupServices_OrderService_OrderServiceId",
                schema: "Orders",
                table: "OrderGroupServices",
                column: "OrderServiceId",
                principalSchema: "Orders",
                principalTable: "OrderService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupServices_OrderGroups_OrderGroupId",
                schema: "Orders",
                table: "OrderGroupServices");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupServices_OrderService_OrderServiceId",
                schema: "Orders",
                table: "OrderGroupServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderGroupServices",
                schema: "Orders",
                table: "OrderGroupServices");

            migrationBuilder.RenameTable(
                name: "OrderGroupServices",
                schema: "Orders",
                newName: "OrderGroupService",
                newSchema: "Orders");

            migrationBuilder.RenameColumn(
                name: "OrderServiceId",
                schema: "Orders",
                table: "OrderGroupService",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderGroupServices_OrderServiceId",
                schema: "Orders",
                table: "OrderGroupService",
                newName: "IX_OrderGroupService_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderGroupServices_OrderGroupId",
                schema: "Orders",
                table: "OrderGroupService",
                newName: "IX_OrderGroupService_OrderGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderGroupService",
                schema: "Orders",
                table: "OrderGroupService",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroupService_OrderGroups_OrderGroupId",
                schema: "Orders",
                table: "OrderGroupService",
                column: "OrderGroupId",
                principalSchema: "Orders",
                principalTable: "OrderGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroupService_Services_ServiceId",
                schema: "Orders",
                table: "OrderGroupService",
                column: "ServiceId",
                principalSchema: "Orders",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
