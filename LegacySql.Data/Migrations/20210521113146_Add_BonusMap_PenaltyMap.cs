using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_BonusMap_PenaltyMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BonusMaps",
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
                    table.PrimaryKey("PK_BonusMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyMaps",
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
                    table.PrimaryKey("PK_PenaltyMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BonusMaps_ErpGuid",
                schema: "public",
                table: "BonusMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonusMaps_LegacyId",
                schema: "public",
                table: "BonusMaps",
                column: "LegacyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyMaps_ErpGuid",
                schema: "public",
                table: "PenaltyMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyMaps_LegacyId",
                schema: "public",
                table: "PenaltyMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BonusMaps",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PenaltyMaps",
                schema: "public");
        }
    }
}
