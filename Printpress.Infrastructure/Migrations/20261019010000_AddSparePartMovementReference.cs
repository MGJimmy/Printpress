using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261019010000_AddSparePartMovementReference")]
    public class AddSparePartMovementReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferenceType",
                schema: "SpareParts",
                table: "SparePartInventoryTransactions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                schema: "SpareParts",
                table: "SparePartInventoryTransactions",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceType",
                schema: "SpareParts",
                table: "SparePartInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                schema: "SpareParts",
                table: "SparePartInventoryTransactions");
        }
    }
}
