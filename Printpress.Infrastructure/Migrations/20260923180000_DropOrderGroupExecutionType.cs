using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260923180000_DropOrderGroupExecutionType")]
    public class DropOrderGroupExecutionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionType",
                schema: "Orders",
                table: "OrderGroups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExecutionType",
                schema: "Orders",
                table: "OrderGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
