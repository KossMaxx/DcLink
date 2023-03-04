using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_ProductSubType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSubtypeMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MapGuid = table.Column<Guid>(nullable: false),
                    ErpGuid = table.Column<Guid>(nullable: true),
                    LegacyId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSubtypeMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubtypeMaps_ErpGuid",
                schema: "public",
                table: "ProductSubtypeMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubtypeMaps_LegacyId",
                schema: "public",
                table: "ProductSubtypeMaps",
                column: "LegacyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubtypeMaps_Title",
                schema: "public",
                table: "ProductSubtypeMaps",
                column: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSubtypeMaps",
                schema: "public");
        }
    }
}
