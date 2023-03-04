using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Change_Index_In_ClientOrderMapEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientOrderMaps_ErpGuid",
                schema: "public",
                table: "ClientOrderMaps");

            migrationBuilder.DropIndex(
                name: "IX_ClientOrderMaps_LegacyId",
                schema: "public",
                table: "ClientOrderMaps");

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrderMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "ClientOrderMaps",
                columns: new[] { "ErpGuid", "LegacyId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientOrderMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "ClientOrderMaps");

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrderMaps_ErpGuid",
                schema: "public",
                table: "ClientOrderMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrderMaps_LegacyId",
                schema: "public",
                table: "ClientOrderMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
