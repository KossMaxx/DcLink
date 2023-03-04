using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_SupplierPriceMap_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierPriceMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MapGuid = table.Column<Guid>(nullable: false),
                    ErpGuid = table.Column<Guid>(nullable: true),
                    LegacyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPriceMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPriceMaps_LegacyId",
                schema: "public",
                table: "SupplierPriceMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierPriceMaps",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_SupplierPriceMaps_LegacyId",
                schema: "public",
                table: "SupplierPriceMaps");
        }
    }
}
