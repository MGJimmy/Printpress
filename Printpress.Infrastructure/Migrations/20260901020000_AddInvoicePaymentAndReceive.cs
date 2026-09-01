using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260901020000_AddInvoicePaymentAndReceive")]
    public class AddInvoicePaymentAndReceive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddSettlementColumns(migrationBuilder, "Inventory", "PurchaseInvoices");
            AddSettlementColumns(migrationBuilder, "SpareParts", "SparePartPurchaseInvoices");

            migrationBuilder.Sql("""
                UPDATE "Inventory"."PurchaseInvoices" SET "PaidAmount" = "TotalAmount", "IsGoodsReceived" = TRUE;
                UPDATE "SpareParts"."SparePartPurchaseInvoices" SET "PaidAmount" = "TotalAmount", "IsGoodsReceived" = TRUE;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropSettlementColumns(migrationBuilder, "Inventory", "PurchaseInvoices");
            DropSettlementColumns(migrationBuilder, "SpareParts", "SparePartPurchaseInvoices");
        }

        private static void AddSettlementColumns(MigrationBuilder migrationBuilder, string schema, string table)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                schema: schema,
                table: table,
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsGoodsReceived",
                schema: schema,
                table: table,
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        private static void DropSettlementColumns(MigrationBuilder migrationBuilder, string schema, string table)
        {
            migrationBuilder.DropColumn(name: "PaidAmount", schema: schema, table: table);
            migrationBuilder.DropColumn(name: "IsGoodsReceived", schema: schema, table: table);
        }
    }
}
