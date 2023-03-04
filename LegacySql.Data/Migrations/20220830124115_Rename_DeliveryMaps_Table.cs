using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Rename_DeliveryMaps_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientOrderDeliveryMaps",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "DeliveryMaps",
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
                    table.PrimaryKey("PK_DeliveryMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryMaps_ErpGuid",
                schema: "public",
                table: "DeliveryMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryMaps_LegacyId",
                schema: "public",
                table: "DeliveryMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryMaps",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "ClientOrderDeliveryMaps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ErpGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    LegacyId = table.Column<int>(type: "integer", nullable: false),
                    MapGuid = table.Column<Guid>(type: "uuid", nullable: false)
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
    }
}
