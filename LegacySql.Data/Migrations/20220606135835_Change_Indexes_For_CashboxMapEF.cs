using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Change_Indexes_For_CashboxMapEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CashboxMaps_ErpGuid",
                schema: "public",
                table: "CashboxMaps");

            migrationBuilder.DropIndex(
                name: "IX_CashboxMaps_LegacyId",
                schema: "public",
                table: "CashboxMaps");

            migrationBuilder.CreateIndex(
                name: "IX_CashboxMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "CashboxMaps",
                columns: new[] { "ErpGuid", "LegacyId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CashboxMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "CashboxMaps");

            migrationBuilder.CreateIndex(
                name: "IX_CashboxMaps_ErpGuid",
                schema: "public",
                table: "CashboxMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashboxMaps_LegacyId",
                schema: "public",
                table: "CashboxMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
