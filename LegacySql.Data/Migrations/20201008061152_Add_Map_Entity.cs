using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_Map_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductTypeCategoryMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MapGuid = table.Column<Guid>(nullable: false),
                    ErpGuid = table.Column<Guid>(nullable: true),
                    LegacyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeCategoryMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypeCategoryParameterMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MapGuid = table.Column<Guid>(nullable: false),
                    ErpGuid = table.Column<Guid>(nullable: true),
                    LegacyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeCategoryParameterMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeCategoryMaps_LegacyId",
                schema: "public",
                table: "ProductTypeCategoryMaps",
                column: "LegacyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeCategoryParameterMaps_LegacyId",
                schema: "public",
                table: "ProductTypeCategoryParameterMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTypeCategoryMaps",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ProductTypeCategoryParameterMaps",
                schema: "public");
        }
    }
}
