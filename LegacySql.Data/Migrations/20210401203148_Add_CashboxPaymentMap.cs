using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Add_CashboxPaymentMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankPaymentMaps",
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
                    table.PrimaryKey("PK_BankPaymentMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashboxPaymentMaps",
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
                    table.PrimaryKey("PK_CashboxPaymentMaps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_ErpGuid",
                schema: "public",
                table: "BankPaymentMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankPaymentMaps_LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                column: "LegacyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashboxPaymentMaps_ErpGuid",
                schema: "public",
                table: "CashboxPaymentMaps",
                column: "ErpGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashboxPaymentMaps_LegacyId",
                schema: "public",
                table: "CashboxPaymentMaps",
                column: "LegacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankPaymentMaps",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CashboxPaymentMaps",
                schema: "public");
        }
    }
}
