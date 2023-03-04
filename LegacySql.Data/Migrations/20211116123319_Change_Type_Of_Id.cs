using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Change_Type_Of_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "WarehouseMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "RelatedProductMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "RejectMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ReconciliationActMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "PurchaseMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeCategoryParameterMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeCategoryMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductSubtypeMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductRefundMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductPriceConditionMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ProductMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "PriceConditionMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "PhysicalPersonMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "InnerId",
                schema: "public",
                table: "NotFullMapped",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "MarketSegmentMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "EmployeeMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "DepartmentMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ClientOrderMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ClientOrderDeliveryMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "ClientMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "CashboxPaymentMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "CashboxMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "WarehouseMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "RelatedProductMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "RejectMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ReconciliationActMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "PurchaseMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeCategoryParameterMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductTypeCategoryMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductSubtypeMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductRefundMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductPriceConditionMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ProductMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "PriceConditionMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "PhysicalPersonMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "InnerId",
                schema: "public",
                table: "NotFullMapped",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "MarketSegmentMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "EmployeeMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "DepartmentMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ClientOrderMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ClientOrderDeliveryMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "ClientMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "CashboxPaymentMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "CashboxMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "LegacyId",
                schema: "public",
                table: "BankPaymentMaps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
