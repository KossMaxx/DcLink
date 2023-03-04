using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Remove_SupplierCurrencyRateMappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierCurrencyRateMaps",
                schema: "public");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierCurrencyRateMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErpGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    LegacyId = table.Column<long>(type: "bigint", nullable: false),
                    MapGuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCurrencyRateMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCurrencyRateMaps_ErpGuid",
                schema: "public",
                table: "SupplierCurrencyRateMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCurrencyRateMaps_LegacyId",
                schema: "public",
                table: "SupplierCurrencyRateMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
