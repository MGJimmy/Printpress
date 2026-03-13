using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addItemStatusAndRenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkerProductions",
                schema: "HR");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemStatus",
                schema: "Orders",
                table: "OrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemServiceExecutions",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemServiceExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemServiceExecutions_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "Orders",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemServiceExecutions_ServiceCategorys_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalSchema: "Orders",
                        principalTable: "ServiceCategorys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemServiceExecutions_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalSchema: "HR",
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemServiceExecutions_OrderItemId",
                schema: "HR",
                table: "ItemServiceExecutions",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemServiceExecutions_ServiceCategoryId",
                schema: "HR",
                table: "ItemServiceExecutions",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemServiceExecutions_WorkerId",
                schema: "HR",
                table: "ItemServiceExecutions",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemServiceExecutions",
                schema: "HR");

            migrationBuilder.DropColumn(
                name: "OrderItemStatus",
                schema: "Orders",
                table: "OrderItems");

            migrationBuilder.CreateTable(
                name: "WorkerProductions",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerProductions_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "Orders",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerProductions_ServiceCategorys_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalSchema: "Orders",
                        principalTable: "ServiceCategorys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerProductions_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalSchema: "HR",
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProductions_OrderItemId",
                schema: "HR",
                table: "WorkerProductions",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProductions_ServiceCategoryId",
                schema: "HR",
                table: "WorkerProductions",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProductions_WorkerId",
                schema: "HR",
                table: "WorkerProductions",
                column: "WorkerId");
        }
    }
}
