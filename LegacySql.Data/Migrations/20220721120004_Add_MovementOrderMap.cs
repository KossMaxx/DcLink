using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Add_MovementOrderMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovementsOrderMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ErpGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    LegacyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementsOrderMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovementsOrderMaps_ErpGuid",
                schema: "public",
                table: "MovementsOrderMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovementsOrderMaps_LegacyId",
                schema: "public",
                table: "MovementsOrderMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovementsOrderMaps",
                schema: "public");
        }
    }
}
