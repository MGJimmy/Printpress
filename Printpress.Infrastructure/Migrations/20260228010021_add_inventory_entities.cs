using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_inventory_entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Client_ClientId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroup_Order_OrderId",
                table: "OrderGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupService_OrderGroup_OrderGroupId",
                table: "OrderGroupService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupService_Service_ServiceId",
                table: "OrderGroupService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderService_Order_OrderId",
                table: "OrderService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderService_Service_ServiceId",
                table: "OrderService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTransaction_Order_OrderId",
                table: "OrderTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceCategory_LKP_ServiceCategoryId",
                table: "Service");

            migrationBuilder.DropTable(
                name: "ItemDetails");

            migrationBuilder.DropTable(
                name: "PrintingServiceDetails");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "PrintingType");

            migrationBuilder.DropTable(
                name: "ProductStock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCategory_LKP",
                table: "ServiceCategory_LKP");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Service",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderTransaction",
                table: "OrderTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderGroup",
                table: "OrderGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDetailsKey_LKP",
                table: "ItemDetailsKey_LKP");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Client",
                table: "Client");

            migrationBuilder.EnsureSchema(
                name: "Orders");

            migrationBuilder.EnsureSchema(
                name: "Inventory");

            migrationBuilder.RenameTable(
                name: "OrderService",
                newName: "OrderService",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "OrderGroupService",
                newName: "OrderGroupService",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "ServiceCategory_LKP",
                newName: "ServiceCategory_LKPs",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "Service",
                newName: "Services",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "OrderTransaction",
                newName: "OrderTransactions",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "OrderGroup",
                newName: "OrderGroups",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "ItemDetailsKey_LKP",
                newName: "ItemDetailsKey_LKPs",
                newSchema: "Orders");

            migrationBuilder.RenameTable(
                name: "Client",
                newName: "Clients",
                newSchema: "Orders");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ServiceCategoryId",
                schema: "Orders",
                table: "Services",
                newName: "IX_Services_ServiceCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderTransaction_OrderId",
                schema: "Orders",
                table: "OrderTransactions",
                newName: "IX_OrderTransactions_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderGroup_OrderId",
                schema: "Orders",
                table: "OrderGroups",
                newName: "IX_OrderGroups_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_ClientId",
                schema: "Orders",
                table: "Orders",
                newName: "IX_Orders_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCategory_LKPs",
                schema: "Orders",
                table: "ServiceCategory_LKPs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                schema: "Orders",
                table: "Services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderTransactions",
                schema: "Orders",
                table: "OrderTransactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderGroups",
                schema: "Orders",
                table: "OrderGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                schema: "Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDetailsKey_LKPs",
                schema: "Orders",
                table: "ItemDetailsKey_LKPs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                schema: "Orders",
                table: "Clients",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InventoryItemCategory_LKPs",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemCategory_LKPs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderGroupId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_OrderGroups_OrderGroupId",
                        column: x => x.OrderGroupId,
                        principalSchema: "Orders",
                        principalTable: "OrderGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    InventoryItemCategoryId = table.Column<int>(type: "integer", nullable: false),
                    PacksPerCarton = table.Column<int>(type: "integer", nullable: true),
                    UnitsPerPack = table.Column<int>(type: "integer", nullable: true),
                    ExpectedPurchaseLossPercent = table.Column<int>(type: "integer", nullable: false),
                    ExpectedProductionWastePercent = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_InventoryItemCategory_LKPs_InventoryItemCate~",
                        column: x => x.InventoryItemCategoryId,
                        principalSchema: "Inventory",
                        principalTable: "InventoryItemCategory_LKPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemDetailss",
                schema: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ItemDetailsKeyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemDetailss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemDetailss_ItemDetailsKey_LKPs_ItemDetailsKeyId",
                        column: x => x.ItemDetailsKeyId,
                        principalSchema: "Orders",
                        principalTable: "ItemDetailsKey_LKPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemDetailss_OrderItems_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Orders",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryItemId = table.Column<int>(type: "integer", nullable: false),
                    InventoryTransactionType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ReferenceType = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalSchema: "Inventory",
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_InventoryItemCategoryId",
                schema: "Inventory",
                table: "InventoryItems",
                column: "InventoryItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_InventoryItemId",
                schema: "Inventory",
                table: "InventoryTransactions",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetailss_ItemDetailsKeyId",
                schema: "Orders",
                table: "OrderItemDetailss",
                column: "ItemDetailsKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemDetailss_ItemId",
                schema: "Orders",
                table: "OrderItemDetailss",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderGroupId",
                schema: "Orders",
                table: "OrderItems",
                column: "OrderGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroups_Orders_OrderId",
                schema: "Orders",
                table: "OrderGroups",
                column: "OrderId",
                principalSchema: "Orders",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Clients_ClientId",
                schema: "Orders",
                table: "Orders",
                column: "ClientId",
                principalSchema: "Orders",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderService_Orders_OrderId",
                schema: "Orders",
                table: "OrderService",
                column: "OrderId",
                principalSchema: "Orders",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderService_Services_ServiceId",
                schema: "Orders",
                table: "OrderService",
                column: "ServiceId",
                principalSchema: "Orders",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTransactions_Orders_OrderId",
                schema: "Orders",
                table: "OrderTransactions",
                column: "OrderId",
                principalSchema: "Orders",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategory_LKPs_ServiceCategoryId",
                schema: "Orders",
                table: "Services",
                column: "ServiceCategoryId",
                principalSchema: "Orders",
                principalTable: "ServiceCategory_LKPs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroups_Orders_OrderId",
                schema: "Orders",
                table: "OrderGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupService_OrderGroups_OrderGroupId",
                schema: "Orders",
                table: "OrderGroupService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderGroupService_Services_ServiceId",
                schema: "Orders",
                table: "OrderGroupService");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Clients_ClientId",
                schema: "Orders",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderService_Orders_OrderId",
                schema: "Orders",
                table: "OrderService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderService_Services_ServiceId",
                schema: "Orders",
                table: "OrderService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTransactions_Orders_OrderId",
                schema: "Orders",
                table: "OrderTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategory_LKPs_ServiceCategoryId",
                schema: "Orders",
                table: "Services");

            migrationBuilder.DropTable(
                name: "InventoryTransactions",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "OrderItemDetailss",
                schema: "Orders");

            migrationBuilder.DropTable(
                name: "InventoryItems",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "Orders");

            migrationBuilder.DropTable(
                name: "InventoryItemCategory_LKPs",
                schema: "Inventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                schema: "Orders",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCategory_LKPs",
                schema: "Orders",
                table: "ServiceCategory_LKPs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderTransactions",
                schema: "Orders",
                table: "OrderTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                schema: "Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderGroups",
                schema: "Orders",
                table: "OrderGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDetailsKey_LKPs",
                schema: "Orders",
                table: "ItemDetailsKey_LKPs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                schema: "Orders",
                table: "Clients");

            migrationBuilder.RenameTable(
                name: "OrderService",
                schema: "Orders",
                newName: "OrderService");

            migrationBuilder.RenameTable(
                name: "OrderGroupService",
                schema: "Orders",
                newName: "OrderGroupService");

            migrationBuilder.RenameTable(
                name: "Services",
                schema: "Orders",
                newName: "Service");

            migrationBuilder.RenameTable(
                name: "ServiceCategory_LKPs",
                schema: "Orders",
                newName: "ServiceCategory_LKP");

            migrationBuilder.RenameTable(
                name: "OrderTransactions",
                schema: "Orders",
                newName: "OrderTransaction");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "OrderGroups",
                schema: "Orders",
                newName: "OrderGroup");

            migrationBuilder.RenameTable(
                name: "ItemDetailsKey_LKPs",
                schema: "Orders",
                newName: "ItemDetailsKey_LKP");

            migrationBuilder.RenameTable(
                name: "Clients",
                schema: "Orders",
                newName: "Client");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServiceCategoryId",
                table: "Service",
                newName: "IX_Service_ServiceCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderTransactions_OrderId",
                table: "OrderTransaction",
                newName: "IX_OrderTransaction_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ClientId",
                table: "Order",
                newName: "IX_Order_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderGroups_OrderId",
                table: "OrderGroup",
                newName: "IX_OrderGroup_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Service",
                table: "Service",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCategory_LKP",
                table: "ServiceCategory_LKP",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderTransaction",
                table: "OrderTransaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderGroup",
                table: "OrderGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDetailsKey_LKP",
                table: "ItemDetailsKey_LKP",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Client",
                table: "Client",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_OrderGroup_OrderGroupId",
                        column: x => x.OrderGroupId,
                        principalTable: "OrderGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintingType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintingType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    StockCategory = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemDetailsKeyId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDetails_ItemDetailsKey_LKP_ItemDetailsKeyId",
                        column: x => x.ItemDetailsKeyId,
                        principalTable: "ItemDetailsKey_LKP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemDetails_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintingServiceDetails",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrintingTypeId = table.Column<int>(type: "integer", nullable: false),
                    ProductStockId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId1 = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintingServiceDetails", x => x.ServiceId);
                    table.ForeignKey(
                        name: "FK_PrintingServiceDetails_PrintingType_PrintingTypeId",
                        column: x => x.PrintingTypeId,
                        principalTable: "PrintingType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintingServiceDetails_ProductStock_ProductStockId",
                        column: x => x.ProductStockId,
                        principalTable: "ProductStock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintingServiceDetails_Service_ServiceId1",
                        column: x => x.ServiceId1,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_OrderGroupId",
                table: "Item",
                column: "OrderGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDetails_ItemDetailsKeyId",
                table: "ItemDetails",
                column: "ItemDetailsKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDetails_ItemId",
                table: "ItemDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintingServiceDetails_PrintingTypeId",
                table: "PrintingServiceDetails",
                column: "PrintingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintingServiceDetails_ProductStockId",
                table: "PrintingServiceDetails",
                column: "ProductStockId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintingServiceDetails_ServiceId1",
                table: "PrintingServiceDetails",
                column: "ServiceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Client_ClientId",
                table: "Order",
                column: "ClientId",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroup_Order_OrderId",
                table: "OrderGroup",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroupService_OrderGroup_OrderGroupId",
                table: "OrderGroupService",
                column: "OrderGroupId",
                principalTable: "OrderGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderGroupService_Service_ServiceId",
                table: "OrderGroupService",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderService_Order_OrderId",
                table: "OrderService",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderService_Service_ServiceId",
                table: "OrderService",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTransaction_Order_OrderId",
                table: "OrderTransaction",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceCategory_LKP_ServiceCategoryId",
                table: "Service",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategory_LKP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
