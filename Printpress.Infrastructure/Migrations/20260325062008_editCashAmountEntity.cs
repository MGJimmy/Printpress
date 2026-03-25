using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editCashAmountEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                schema: "General",
                table: "CashTransactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "General",
                table: "CashAccounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "General",
                table: "CashAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "General",
                table: "CashAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "General",
                table: "CashAccounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "General",
                table: "CashAccounts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionDate",
                schema: "General",
                table: "CashTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "General",
                table: "CashAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "General",
                table: "CashAccounts");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "General",
                table: "CashAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "General",
                table: "CashAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "General",
                table: "CashAccounts");
        }
    }
}
