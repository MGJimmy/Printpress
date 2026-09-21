using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260921190000_AddOrderServiceIsCover")]
    public class AddOrderServiceIsCover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCover",
                schema: "Orders",
                table: "OrderService",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE "Orders"."OrderService" os
                SET "IsCover" = true
                WHERE os."IsDeleted" = false
                  AND EXISTS (
                      SELECT 1
                      FROM "Orders"."OrderGroupService" ogs
                      JOIN "Orders"."OrderGroups" og ON og."Id" = ogs."OrderGroupId"
                      WHERE og."OrderId" = os."OrderId"
                        AND ogs."ServiceId" = os."ServiceId"
                        AND ogs."IsCover" = true
                        AND ogs."IsDeleted" = false
                        AND og."IsDeleted" = false
                  )
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "Orders"."OrderGroupService" ogs2
                      JOIN "Orders"."OrderGroups" og2 ON og2."Id" = ogs2."OrderGroupId"
                      WHERE og2."OrderId" = os."OrderId"
                        AND ogs2."ServiceId" = os."ServiceId"
                        AND ogs2."IsCover" = false
                        AND ogs2."IsDeleted" = false
                        AND og2."IsDeleted" = false
                  );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCover",
                schema: "Orders",
                table: "OrderService");
        }
    }
}
