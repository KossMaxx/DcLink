using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Add_IncommingBilsMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMovings",
                schema: "public",
                table: "ProductMovings");

            migrationBuilder.RenameTable(
                name: "ProductMovings",
                schema: "public",
                newName: "ProductMovingMaps",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_ProductMovings_LegacyId",
                schema: "public",
                table: "ProductMovingMaps",
                newName: "IX_ProductMovingMaps_LegacyId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductMovings_ErpGuid",
                schema: "public",
                table: "ProductMovingMaps",
                newName: "IX_ProductMovingMaps_ErpGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMovingMaps",
                schema: "public",
                table: "ProductMovingMaps",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IncomingBilsMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ErpGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    LegacyId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingBilsMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingBilsMaps_ErpGuid",
                schema: "public",
                table: "IncomingBilsMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomingBilsMaps_LegacyId",
                schema: "public",
                table: "IncomingBilsMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingBilsMaps",
                schema: "public");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMovingMaps",
                schema: "public",
                table: "ProductMovingMaps");

            migrationBuilder.RenameTable(
                name: "ProductMovingMaps",
                schema: "public",
                newName: "ProductMovings",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_ProductMovingMaps_LegacyId",
                schema: "public",
                table: "ProductMovings",
                newName: "IX_ProductMovings_LegacyId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductMovingMaps_ErpGuid",
                schema: "public",
                table: "ProductMovings",
                newName: "IX_ProductMovings_ErpGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMovings",
                schema: "public",
                table: "ProductMovings",
                column: "Id");
        }
    }
}
