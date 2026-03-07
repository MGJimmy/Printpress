using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class generalUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                schema: "Inventory",
                table: "PurchaseInvoiceLines",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "Inventory",
                table: "PurchaseInvoiceLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

        }
    }
}
