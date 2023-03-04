using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_Index_To_ErpNotFullMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ErpNotFullMapped_ErpId_Type",
                schema: "public",
                table: "ErpNotFullMapped",
                columns: new[] { "ErpId", "Type" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ErpNotFullMapped_ErpId_Type",
                schema: "public",
                table: "ErpNotFullMapped");
        }
    }
}
