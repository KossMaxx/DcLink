using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Change_BankPaymentMapEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankPaymentMaps_ErpGuid",
                schema: "public",
                table: "BankPaymentMaps");

            migrationBuilder.AddColumn<int>(
                name: "ClientOrderId",
                schema: "public",
                table: "BankPaymentMaps",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankPaymentMaps_LegacyId",
                schema: "public",
                table: "BankPaymentMaps");

            migrationBuilder.DropColumn(
                name: "ClientOrderId",
                schema: "public",
                table: "BankPaymentMaps");

            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_ErpGuid",
                schema: "public",
                table: "BankPaymentMaps",
                column: "ErpGuid",
                unique: true);
        }
    }
}
