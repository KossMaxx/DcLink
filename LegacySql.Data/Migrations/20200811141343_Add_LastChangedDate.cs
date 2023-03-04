using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_LastChangedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LastChangedDates",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EntityType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastChangedDates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrderMaps_LegacyId",
                schema: "public",
                table: "ClientOrderMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastChangedDates",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_ClientOrderMaps_LegacyId",
                schema: "public",
                table: "ClientOrderMaps");
        }
    }
}
