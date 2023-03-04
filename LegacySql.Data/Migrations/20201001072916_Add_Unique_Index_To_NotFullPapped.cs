using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_Unique_Index_To_NotFullPapped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NotFullMapped_InnerId_Type",
                schema: "public",
                table: "NotFullMapped",
                columns: new[] { "InnerId", "Type" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotFullMapped_InnerId_Type",
                schema: "public",
                table: "NotFullMapped");
        }
    }
}
