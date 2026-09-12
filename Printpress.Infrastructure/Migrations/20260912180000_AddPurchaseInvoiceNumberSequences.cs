using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Printpress.Infrastructure;

#nullable disable

namespace Printpress.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260912180000_AddPurchaseInvoiceNumberSequences")]
    public class AddPurchaseInvoiceNumberSequences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ConvertInvoiceNumberToSequence(
                migrationBuilder,
                schema: "Inventory",
                table: "PurchaseInvoices",
                sequence: "PurchaseInvoiceNumber");

            ConvertInvoiceNumberToSequence(
                migrationBuilder,
                schema: "SpareParts",
                table: "SparePartPurchaseInvoices",
                sequence: "SparePartPurchaseInvoiceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RevertInvoiceNumberToString(migrationBuilder, "Inventory", "PurchaseInvoices", "PurchaseInvoiceNumber");
            RevertInvoiceNumberToString(migrationBuilder, "SpareParts", "SparePartPurchaseInvoices", "SparePartPurchaseInvoiceNumber");
        }

        private static void ConvertInvoiceNumberToSequence(
            MigrationBuilder migrationBuilder,
            string schema,
            string table,
            string sequence)
        {
            migrationBuilder.CreateSequence(
                name: sequence,
                schema: schema,
                startValue: 1L,
                incrementBy: 1);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceNumberSeq",
                schema: schema,
                table: table,
                type: "integer",
                nullable: true);

            migrationBuilder.Sql($"""
                UPDATE "{schema}"."{table}"
                SET "InvoiceNumberSeq" = "InvoiceNumber"::integer
                WHERE "InvoiceNumber" ~ '^[0-9]+$';

                WITH numbered AS (
                    SELECT "Id", ROW_NUMBER() OVER (ORDER BY "CreatedAt", "Id") AS rn
                    FROM "{schema}"."{table}"
                    WHERE "InvoiceNumberSeq" IS NULL
                ),
                max_existing AS (
                    SELECT COALESCE(MAX("InvoiceNumberSeq"), 0) AS m
                    FROM "{schema}"."{table}"
                )
                UPDATE "{schema}"."{table}" i
                SET "InvoiceNumberSeq" = n.rn + m.m
                FROM numbered n, max_existing m
                WHERE i."Id" = n."Id";
                """);

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                schema: schema,
                table: table);

            migrationBuilder.RenameColumn(
                name: "InvoiceNumberSeq",
                schema: schema,
                table: table,
                newName: "InvoiceNumber");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceNumber",
                schema: schema,
                table: table,
                type: "integer",
                nullable: false,
                defaultValueSql: $"nextval('\"{schema}\".\"{sequence}\"')");

            migrationBuilder.Sql($"""
                DO $$
                DECLARE m integer;
                BEGIN
                    SELECT COALESCE(MAX("InvoiceNumber"), 0) INTO m FROM "{schema}"."{table}";
                    IF m = 0 THEN
                        PERFORM setval('"{schema}"."{sequence}"', 1, false);
                    ELSE
                        PERFORM setval('"{schema}"."{sequence}"', m, true);
                    END IF;
                END $$;
                """);
        }

        private static void RevertInvoiceNumberToString(
            MigrationBuilder migrationBuilder,
            string schema,
            string table,
            string sequence)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InvoiceNumber",
                schema: schema,
                table: table,
                type: "integer",
                nullable: false,
                oldDefaultValueSql: $"nextval('\"{schema}\".\"{sequence}\"')");

            migrationBuilder.Sql($"""
                ALTER TABLE "{schema}"."{table}"
                ALTER COLUMN "InvoiceNumber" DROP DEFAULT;

                ALTER TABLE "{schema}"."{table}"
                ALTER COLUMN "InvoiceNumber" TYPE character varying(100)
                USING "InvoiceNumber"::text;
                """);

            migrationBuilder.DropSequence(name: sequence, schema: schema);
        }
    }
}
