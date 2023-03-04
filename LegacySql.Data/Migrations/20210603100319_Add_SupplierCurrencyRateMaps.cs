using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_SupplierCurrencyRateMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierCurrencyRateMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MapGuid = table.Column<Guid>(nullable: false),
                    ErpGuid = table.Column<Guid>(nullable: true),
                    LegacyId = table.Column<long>(nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierCurrencyRateMaps",
                schema: "public");
        }
    }
}
