using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Change_Fields_Types_In_MapEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "SupplierPriceMaps",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeMaps",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductMaps",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ClientOrderMaps",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ClientMaps",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "SupplierPriceMaps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeMaps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductMaps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ClientOrderMaps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ClientMaps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
