using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class orderGroupEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderGroup");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryNotes",
                table: "OrderGroup",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryNotes",
                table: "OrderGroup");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrderGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
