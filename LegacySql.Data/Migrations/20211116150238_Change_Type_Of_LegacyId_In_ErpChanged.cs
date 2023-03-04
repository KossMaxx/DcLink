using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Change_Type_Of_LegacyId_In_ErpChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ErpChanged",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ErpChanged",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
