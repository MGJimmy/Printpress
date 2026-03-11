using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSparePartsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SpareParts");

            migrationBuilder.CreateSequence<int>(
                name: "SparePartSellingInvoiceNumber",
                schema: "SpareParts");

            migrationBuilder.CreateTable(
                name: "SparePartInventoryItems",
                schema: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PacksPerCarton = table.Column<int>(type: "integer", nullable: true),
                    UnitsPerPack = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartInventoryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SparePartPurchaseInvoices",
                schema: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SupplierName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AttachmentFilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartPurchaseInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SparePartSellingInvoices",
                schema: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"SpareParts\".\"SparePartSellingInvoiceNumber\"')"),
                    ClientName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartSellingInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SparePartInventoryTransactions",
                schema: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryTransactionType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartInventoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartInventoryTransactions_SparePartInventoryItems_Inve~",
                        column: x => x.InventoryItemId,
                        principalSchema: "SpareParts",
                        principalTable: "SparePartInventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartPurchaseInvoiceLines",
                schema: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartPurchaseInvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartPurchaseInvoiceLines_SparePartInventoryItems_Inven~",
                        column: x => x.InventoryItemId,
                        principalSchema: "SpareParts",
                        principalTable: "SparePartInventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SparePartPurchaseInvoiceLines_SparePartPurchaseInvoices_Pur~",
                        column: x => x.PurchaseInvoiceId,
                        principalSchema: "SpareParts",
                        principalTable: "SparePartPurchaseInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartSellingInvoiceLines",
                schema: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SellingInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartSellingInvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartSellingInvoiceLines_SparePartInventoryItems_Invent~",
                        column: x => x.InventoryItemId,
                        principalSchema: "SpareParts",
                        principalTable: "SparePartInventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SparePartSellingInvoiceLines_SparePartSellingInvoices_Selli~",
                        column: x => x.SellingInvoiceId,
                        principalSchema: "SpareParts",
                        principalTable: "SparePartSellingInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SparePartInventoryTransactions_InventoryItemId",
                schema: "SpareParts",
                table: "SparePartInventoryTransactions",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartPurchaseInvoiceLines_InventoryItemId",
                schema: "SpareParts",
                table: "SparePartPurchaseInvoiceLines",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartPurchaseInvoiceLines_PurchaseInvoiceId",
                schema: "SpareParts",
                table: "SparePartPurchaseInvoiceLines",
                column: "PurchaseInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartSellingInvoiceLines_InventoryItemId",
                schema: "SpareParts",
                table: "SparePartSellingInvoiceLines",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartSellingInvoiceLines_SellingInvoiceId",
                schema: "SpareParts",
                table: "SparePartSellingInvoiceLines",
                column: "SellingInvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SparePartInventoryTransactions",
                schema: "SpareParts");

            migrationBuilder.DropTable(
                name: "SparePartPurchaseInvoiceLines",
                schema: "SpareParts");

            migrationBuilder.DropTable(
                name: "SparePartSellingInvoiceLines",
                schema: "SpareParts");

            migrationBuilder.DropTable(
                name: "SparePartPurchaseInvoices",
                schema: "SpareParts");

            migrationBuilder.DropTable(
                name: "SparePartInventoryItems",
                schema: "SpareParts");

            migrationBuilder.DropTable(
                name: "SparePartSellingInvoices",
                schema: "SpareParts");

            migrationBuilder.DropSequence(
                name: "SparePartSellingInvoiceNumber",
                schema: "SpareParts");
        }
    }
}
