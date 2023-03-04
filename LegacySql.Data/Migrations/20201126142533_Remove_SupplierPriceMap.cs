using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Remove_SupplierPriceMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierPriceMaps",
                schema: "public");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierPriceMaps",
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
                    table.PrimaryKey("PK_SupplierPriceMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPriceMaps_ErpGuid",
                schema: "public",
                table: "SupplierPriceMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPriceMaps_LegacyId",
                schema: "public",
                table: "SupplierPriceMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
