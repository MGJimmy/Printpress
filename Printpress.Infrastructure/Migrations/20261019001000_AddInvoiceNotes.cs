using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261019001000_AddInvoiceNotes")]
    public class AddInvoiceNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "Inventory",
                table: "PurchaseInvoices",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "SpareParts",
                table: "SparePartPurchaseInvoices",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "SpareParts",
                table: "SparePartSellingInvoices",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "Inventory",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "SpareParts",
                table: "SparePartPurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "SpareParts",
                table: "SparePartSellingInvoices");
        }
    }
}
