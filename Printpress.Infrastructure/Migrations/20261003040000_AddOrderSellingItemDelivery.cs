using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261003040000_AddOrderSellingItemDelivery")]
    public class AddOrderSellingItemDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                schema: "Orders",
                table: "OrderSellingItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                schema: "Orders",
                table: "OrderSellingItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryName",
                schema: "Orders",
                table: "OrderSellingItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                schema: "Orders",
                table: "OrderSellingItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryNotes",
                schema: "Orders",
                table: "OrderSellingItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelivered",
                schema: "Orders",
                table: "OrderSellingItems");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                schema: "Orders",
                table: "OrderSellingItems");

            migrationBuilder.DropColumn(
                name: "DeliveryName",
                schema: "Orders",
                table: "OrderSellingItems");

            migrationBuilder.DropColumn(
                name: "ReceiverName",
                schema: "Orders",
                table: "OrderSellingItems");

            migrationBuilder.DropColumn(
                name: "DeliveryNotes",
                schema: "Orders",
                table: "OrderSellingItems");
        }
    }
}
