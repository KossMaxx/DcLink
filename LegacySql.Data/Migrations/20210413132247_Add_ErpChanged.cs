using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_ErpChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErpChanged",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LegacyId = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErpChanged", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErpChanged_LegacyId_Type",
                schema: "public",
                table: "ErpChanged",
                columns: new[] { "LegacyId", "Type" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErpChanged",
                schema: "public");
        }
    }
}
