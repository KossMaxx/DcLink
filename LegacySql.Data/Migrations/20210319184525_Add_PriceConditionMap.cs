using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_PriceConditionMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceConditionMaps",
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
                    table.PrimaryKey("PK_PriceConditionMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceConditionMaps_ErpGuid",
                schema: "public",
                table: "PriceConditionMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceConditionMaps_LegacyId",
                schema: "public",
                table: "PriceConditionMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceConditionMaps",
                schema: "public");
        }
    }
}
