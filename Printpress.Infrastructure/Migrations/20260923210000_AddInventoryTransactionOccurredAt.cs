using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260923210000_AddInventoryTransactionOccurredAt")]
    public class AddInventoryTransactionOccurredAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OccurredAt",
                schema: "Inventory",
                table: "InventoryTransactions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(
                """UPDATE "Inventory"."InventoryTransactions" SET "OccurredAt" = "CreatedAt" WHERE "OccurredAt" IS NULL;""");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurredAt",
                schema: "Inventory",
                table: "InventoryTransactions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OccurredAt",
                schema: "Inventory",
                table: "InventoryTransactions");
        }
    }
}
