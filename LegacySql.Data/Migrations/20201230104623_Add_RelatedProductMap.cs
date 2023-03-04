using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_RelatedProductMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelatedProductMaps",
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
                    table.PrimaryKey("PK_RelatedProductMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelatedProductMaps_ErpGuid",
                schema: "public",
                table: "RelatedProductMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedProductMaps_LegacyId",
                schema: "public",
                table: "RelatedProductMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelatedProductMaps",
                schema: "public");
        }
    }
}
