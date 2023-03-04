using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Set_NotUnique_Index_For_NotFullMapped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotFullMapped_Type",
                schema: "public",
                table: "NotFullMapped");

            migrationBuilder.CreateIndex(
                name: "IX_NotFullMapped_Type",
                schema: "public",
                table: "NotFullMapped",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotFullMapped_Type",
                schema: "public",
                table: "NotFullMapped");

            migrationBuilder.CreateIndex(
                name: "IX_NotFullMapped_Type",
                schema: "public",
                table: "NotFullMapped",
                column: "Type",
                unique: true);
        }
    }
}
