using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260901010000_AddInvoiceVoid")]
    public class AddInvoiceVoid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddVoidColumns(migrationBuilder, "Inventory", "PurchaseInvoices");
            AddVoidColumns(migrationBuilder, "SpareParts", "SparePartPurchaseInvoices");
            AddVoidColumns(migrationBuilder, "SpareParts", "SparePartSellingInvoices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropVoidColumns(migrationBuilder, "Inventory", "PurchaseInvoices");
            DropVoidColumns(migrationBuilder, "SpareParts", "SparePartPurchaseInvoices");
            DropVoidColumns(migrationBuilder, "SpareParts", "SparePartSellingInvoices");
        }

        private static void AddVoidColumns(MigrationBuilder migrationBuilder, string schema, string table)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                schema: schema,
                table: table,
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoidReason",
                schema: schema,
                table: table,
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoidedAt",
                schema: schema,
                table: table,
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoidedBy",
                schema: schema,
                table: table,
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        private static void DropVoidColumns(MigrationBuilder migrationBuilder, string schema, string table)
        {
            migrationBuilder.DropColumn(name: "IsVoided", schema: schema, table: table);
            migrationBuilder.DropColumn(name: "VoidReason", schema: schema, table: table);
            migrationBuilder.DropColumn(name: "VoidedAt", schema: schema, table: table);
            migrationBuilder.DropColumn(name: "VoidedBy", schema: schema, table: table);
        }
    }
}
