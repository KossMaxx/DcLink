using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_ProductMap_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "ProductMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MapGuid = table.Column<Guid>(nullable: false),
                    ErpGuid = table.Column<Guid>(nullable: true),
                    LegacyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMaps_LegacyId",
                schema: "public",
                table: "ProductMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMaps",
                schema: "public");
        }
    }
}
