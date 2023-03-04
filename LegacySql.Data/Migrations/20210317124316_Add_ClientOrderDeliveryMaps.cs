using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_ClientOrderDeliveryMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientOrderDeliveryMaps",
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
                    table.PrimaryKey("PK_ClientOrderDeliveryMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrderDeliveryMaps_ErpGuid",
                schema: "public",
                table: "ClientOrderDeliveryMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrderDeliveryMaps_LegacyId",
                schema: "public",
                table: "ClientOrderDeliveryMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientOrderDeliveryMaps",
                schema: "public");
        }
    }
}
