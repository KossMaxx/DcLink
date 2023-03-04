using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_SegmentationTurnoversMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SegmentationTurnoverMaps",
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
                    table.PrimaryKey("PK_SegmentationTurnoverMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SegmentationTurnoverMaps_ErpGuid",
                schema: "public",
                table: "SegmentationTurnoverMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SegmentationTurnoverMaps_LegacyId",
                schema: "public",
                table: "SegmentationTurnoverMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SegmentationTurnoverMaps",
                schema: "public");
        }
    }
}
