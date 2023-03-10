using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Change_Indexes_For_BankPaymentMapEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankPaymentMaps_LegacyId",
                schema: "public",
                table: "BankPaymentMaps");

            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                columns: new[] { "ErpGuid", "LegacyId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankPaymentMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "BankPaymentMaps");

            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
