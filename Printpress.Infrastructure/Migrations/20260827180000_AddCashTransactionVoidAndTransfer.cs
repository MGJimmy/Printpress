using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260827180000_AddCashTransactionVoidAndTransfer")]
    public class AddCashTransactionVoidAndTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                schema: "General",
                table: "CashTransactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ReversesTransactionId",
                schema: "General",
                table: "CashTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashTransactions_ReversesTransactionId",
                schema: "General",
                table: "CashTransactions",
                column: "ReversesTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashTransactions_CashTransactions_ReversesTransactionId",
                schema: "General",
                table: "CashTransactions",
                column: "ReversesTransactionId",
                principalSchema: "General",
                principalTable: "CashTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashTransactions_CashTransactions_ReversesTransactionId",
                schema: "General",
                table: "CashTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CashTransactions_ReversesTransactionId",
                schema: "General",
                table: "CashTransactions");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                schema: "General",
                table: "CashTransactions");

            migrationBuilder.DropColumn(
                name: "ReversesTransactionId",
                schema: "General",
                table: "CashTransactions");
        }
    }
}
