using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacySql.Data.Migrations
{
    public partial class Add_CreateDate_Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "WarehouseMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "SegmentationTurnoverMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "RelatedProductMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "RejectMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ReconciliationActMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "PurchaseMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeCategoryParameterMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeCategoryMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeCategoryGroupMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductSubtypeMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductRefundMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductPriceConditionMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ProductMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "PriceConditionMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "PhysicalPersonMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "PaymentOrderMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "PartnerProductGroupMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "MovementsOrderMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "MarketSegmentMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "FreeDocumentMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "FirmMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "EmployeeMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "DepartmentMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ClientOrderMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ClientOrderDeliveryMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ClientMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "CashboxPaymentMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "CashboxMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "CashboxApplicationPaymentMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "BillMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "BankPaymentMaps",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "public",
                table: "ActivityTypes",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: DateTime.Now);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "WarehouseMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "SegmentationTurnoverMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "RelatedProductMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "RejectMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ReconciliationActMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "PurchaseMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeCategoryParameterMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeCategoryMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductTypeCategoryGroupMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductSubtypeMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductRefundMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductPriceConditionMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ProductMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "PriceConditionMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "PhysicalPersonMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "PaymentOrderMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "PartnerProductGroupMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "MovementsOrderMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "MarketSegmentMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "FreeDocumentMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "FirmMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "EmployeeMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "DepartmentMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ClientOrderMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ClientOrderDeliveryMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ClientMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "CashboxPaymentMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "CashboxMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "CashboxApplicationPaymentMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "BillMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "BankPaymentMaps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "public",
                table: "ActivityTypes");
        }
    }
}
