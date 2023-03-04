using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_Date_Why_Fieldt_To_NotFullMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                schema: "public",
                table: "NotFullMapped",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Why",
                schema: "public",
                table: "NotFullMapped",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                schema: "public",
                table: "NotFullMapped");

            migrationBuilder.DropColumn(
                name: "Why",
                schema: "public",
                table: "NotFullMapped");
        }
    }
}
