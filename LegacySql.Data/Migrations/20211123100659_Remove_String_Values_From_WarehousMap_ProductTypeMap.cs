using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Remove_String_Values_From_WarehousMap_ProductTypeMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "WarehouseMaps");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "public",
                table: "ProductTypeMaps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "WarehouseMaps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "public",
                table: "ProductTypeMaps",
                type: "text",
                nullable: true);
        }
    }
}
