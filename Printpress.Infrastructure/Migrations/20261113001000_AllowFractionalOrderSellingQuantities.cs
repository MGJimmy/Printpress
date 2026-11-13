using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Printpress.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261113001000_AllowFractionalOrderSellingQuantities")]
public class AllowFractionalOrderSellingQuantities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<double>(
            name: "Quantity", schema: "Orders", table: "OrderSellingItems",
            type: "double precision", nullable: false,
            oldClrType: typeof(int), oldType: "integer");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Refuse a downgrade that would silently round existing fractional quantities.
        migrationBuilder.Sql("""
            DO $$ BEGIN
                IF EXISTS (SELECT 1 FROM "Orders"."OrderSellingItems"
                           WHERE "Quantity" <> trunc("Quantity")) THEN
                    RAISE EXCEPTION 'Cannot restore integer quantities while fractional selling quantities exist.';
                END IF;
            END $$;
            """);
        migrationBuilder.AlterColumn<int>(
            name: "Quantity", schema: "Orders", table: "OrderSellingItems",
            type: "integer", nullable: false,
            oldClrType: typeof(double), oldType: "double precision");
    }
}
