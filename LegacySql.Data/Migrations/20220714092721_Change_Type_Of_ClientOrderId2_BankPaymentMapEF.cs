using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Change_Type_Of_ClientOrderId2_BankPaymentMapEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankPaymentMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "BankPaymentMaps");

            migrationBuilder.DropIndex(
                name: "IX_BankPaymentMaps_LegacyId",
                schema: "public",
                table: "BankPaymentMaps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_ErpGuid_LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                columns: new[] { "ErpGuid", "LegacyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
