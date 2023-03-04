using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Remove_Bonuses_Penalties_Maps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BonusMaps",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PenaltyMaps",
                schema: "public");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BonusMaps",
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
                    table.PrimaryKey("PK_BonusMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyMaps",
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
    }
}
