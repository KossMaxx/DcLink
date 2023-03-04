using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Remove_WarehouseStockMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseStockMaps",
                schema: "public");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarehouseStockMaps",
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
                    table.PrimaryKey("PK_WarehouseStockMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStockMaps_ErpGuid",
                schema: "public",
                table: "WarehouseStockMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStockMaps_LegacyId",
                schema: "public",
                table: "WarehouseStockMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
