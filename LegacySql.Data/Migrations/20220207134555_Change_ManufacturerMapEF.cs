using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Change_ManufacturerMapEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ManufacturerMaps_LegacyTitle",
                schema: "public",
                table: "ManufacturerMaps");

            migrationBuilder.AddColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ManufacturerMaps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturerMaps_LegacyId",
                schema: "public",
                table: "ManufacturerMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ManufacturerMaps_LegacyId",
                schema: "public",
                table: "ManufacturerMaps");

            migrationBuilder.DropColumn(
                name: "LegacyId",
                schema: "public",
                table: "ManufacturerMaps");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturerMaps_LegacyTitle",
                schema: "public",
                table: "ManufacturerMaps",
                column: "LegacyTitle",
                unique: true);
        }
    }
}
