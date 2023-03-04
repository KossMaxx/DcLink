using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_Index_By_ErpGuid_To_ProductMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductMaps_ErpGuid",
                schema: "public",
                table: "ProductMaps",
                column: "ErpGuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductMaps_ErpGuid",
                schema: "public",
                table: "ProductMaps");
        }
    }
}
