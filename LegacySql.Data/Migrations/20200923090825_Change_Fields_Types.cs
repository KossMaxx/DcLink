using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Change_Fields_Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "InnerId",
                schema: "public",
                table: "NotFullMapped",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InnerId",
                schema: "public",
                table: "NotFullMapped",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
