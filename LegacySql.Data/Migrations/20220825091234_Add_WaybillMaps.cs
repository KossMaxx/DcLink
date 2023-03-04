using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Add_WaybillMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaybillMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ErpGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    LegacyId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaybillMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaybillMaps_ErpGuid",
                schema: "public",
                table: "WaybillMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaybillMaps_LegacyId",
                schema: "public",
                table: "WaybillMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaybillMaps",
                schema: "public");
        }
    }
}
