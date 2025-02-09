using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class serviceCategoryLKP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceCategory",
                table: "Service",
                newName: "ServiceCategoryId");

            migrationBuilder.CreateTable(
                name: "ServiceCategory_LKP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategory_LKP", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceCategoryId",
                table: "Service",
                column: "ServiceCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceCategory_LKP_ServiceCategoryId",
                table: "Service",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategory_LKP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceCategory_LKP_ServiceCategoryId",
                table: "Service");

            migrationBuilder.DropTable(
                name: "ServiceCategory_LKP");

            migrationBuilder.DropIndex(
                name: "IX_Service_ServiceCategoryId",
                table: "Service");

            migrationBuilder.RenameColumn(
                name: "ServiceCategoryId",
                table: "Service",
                newName: "ServiceCategory");
        }
    }
}
