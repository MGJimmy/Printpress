using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class itemDetailsLKP_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "ItemDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ItemDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemDetailsKeyId",
                table: "ItemDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemDetailsKey_LKP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDetailsKey_LKP", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemDetails_ItemDetailsKeyId",
                table: "ItemDetails",
                column: "ItemDetailsKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDetails_ItemDetailsKey_LKP_ItemDetailsKeyId",
                table: "ItemDetails",
                column: "ItemDetailsKeyId",
                principalTable: "ItemDetailsKey_LKP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemDetails_ItemDetailsKey_LKP_ItemDetailsKeyId",
                table: "ItemDetails");

            migrationBuilder.DropTable(
                name: "ItemDetailsKey_LKP");

            migrationBuilder.DropIndex(
                name: "IX_ItemDetails_ItemDetailsKeyId",
                table: "ItemDetails");

            migrationBuilder.DropColumn(
                name: "ItemDetailsKeyId",
                table: "ItemDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ItemDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ItemDetails",
                type: "text",
                nullable: true);
        }
    }
}
