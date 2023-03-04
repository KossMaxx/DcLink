using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_Index_By_ErpGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SellingPriceMaps_LegacyId",
                schema: "public",
                table: "SellingPriceMaps");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPriceMaps_ErpGuid",
                schema: "public",
                table: "SupplierPriceMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeMaps_ErpGuid",
                schema: "public",
                table: "ProductTypeMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeCategoryParameterMaps_ErpGuid",
                schema: "public",
                table: "ProductTypeCategoryParameterMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeCategoryMaps_ErpGuid",
                schema: "public",
                table: "ProductTypeCategoryMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrderMaps_ErpGuid",
                schema: "public",
                table: "ClientOrderMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientMaps_ErpGuid",
                schema: "public",
                table: "ClientMaps",
                column: "ErpGuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplierPriceMaps_ErpGuid",
                schema: "public",
                table: "SupplierPriceMaps");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypeMaps_ErpGuid",
                schema: "public",
                table: "ProductTypeMaps");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypeCategoryParameterMaps_ErpGuid",
                schema: "public",
                table: "ProductTypeCategoryParameterMaps");

            migrationBuilder.DropIndex(
                name: "IX_ProductTypeCategoryMaps_ErpGuid",
                schema: "public",
                table: "ProductTypeCategoryMaps");

            migrationBuilder.DropIndex(
                name: "IX_ClientOrderMaps_ErpGuid",
                schema: "public",
                table: "ClientOrderMaps");

            migrationBuilder.DropIndex(
                name: "IX_ClientMaps_ErpGuid",
                schema: "public",
                table: "ClientMaps");

            migrationBuilder.CreateIndex(
                name: "IX_SellingPriceMaps_LegacyId",
                schema: "public",
                table: "SellingPriceMaps",
                column: "LegacyId",
                unique: true);
        }
    }
}
