using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_ProductPriceConditionMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductPriceConditionMaps",
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
                    table.PrimaryKey("PK_ProductPriceConditionMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceConditionMaps_ErpGuid",
                schema: "public",
                table: "ProductPriceConditionMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceConditionMaps_LegacyId",
                schema: "public",
                table: "ProductPriceConditionMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPriceConditionMaps",
                schema: "public");
        }
    }
}
