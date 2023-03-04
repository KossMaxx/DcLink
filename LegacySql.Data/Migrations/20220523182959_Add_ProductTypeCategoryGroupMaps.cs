using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Add_ProductTypeCategoryGroupMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductTypeCategoryGroupMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ErpGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    LegacyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeCategoryGroupMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeCategoryGroupMaps_ErpGuid",
                schema: "public",
                table: "ProductTypeCategoryGroupMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeCategoryGroupMaps_LegacyId",
                schema: "public",
                table: "ProductTypeCategoryGroupMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTypeCategoryGroupMaps",
                schema: "public");
        }
    }
}
