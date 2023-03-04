using System;

namespace LegacySql.Domain.Extensions
{
    public static class TypeExtensions
    {
        public static string GetEntityName(this Type type)
        {
            var typeName = type.Name;

            if (typeName.IndexOf("ReconciliationAct", StringComparison.Ordinal) != -1)
            {
                return "reconciliation-acts";
            }

            if (typeName.IndexOf("Bonus", StringComparison.Ordinal) != -1)
            {
                return "bonuses";
            }

            if (typeName.IndexOf("Penalty", StringComparison.Ordinal) != -1)
            {
                return "penalties";
            }

            if (typeName.IndexOf("ProductSubtype", StringComparison.Ordinal) != -1)
            {
                return "product-subtypes";
            }

            if (typeName.IndexOf("MarketSegment", StringComparison.Ordinal) != -1)
            {
                return "market-segments";
            }

            if (typeName.IndexOf("Manufacturer", StringComparison.Ordinal) != -1)
            {
                return "manufacturers";
            }

            if (typeName.IndexOf("CashboxPayment", StringComparison.Ordinal) != -1)
            {
                return "cashbox-payments";
            }

            if (typeName.IndexOf("BankPayment", StringComparison.Ordinal) != -1)
            {
                return "bank-payments";
            }

            if (typeName.IndexOf("Reject", StringComparison.Ordinal) != -1)
            {
                return "rejects";
            }

            if (typeName.IndexOf("Purchase", StringComparison.Ordinal) != -1)
            {
                return "purchases";
            }

            if (typeName.IndexOf("WarehouseStock", StringComparison.Ordinal) != -1)
            {
                return "warehouse-stocks";
            }

            if (typeName.IndexOf("PhysicalPerson", StringComparison.Ordinal) != -1)
            {
                return "physical-persons";
            }

            if (typeName.IndexOf("ClientOrder", StringComparison.Ordinal) != -1)
            {
                return "client-orders";
            }

            if (typeName.IndexOf("Client", StringComparison.Ordinal) != -1)
            {
                return "clients";
            }

            if (typeName.IndexOf("RelatedProduct", StringComparison.Ordinal) != -1)
            {
                return "related-products";
            }

            if (typeName.IndexOf("ProductPriceCondition", StringComparison.Ordinal) != -1)
            {
                return "product-price-conditions";
            }

            if (typeName.IndexOf("ProductRefund", StringComparison.Ordinal) != -1)
            {
                return "product-refunds";
            }
            if (typeName.IndexOf("ProductTypeCategoryGroup", StringComparison.Ordinal) != -1)
            {
                return "product-type-category-groups";
            }

            if (typeName.IndexOf("ProductType", StringComparison.Ordinal) != -1)
            {
                return "product-types";
            }

            if (typeName.IndexOf("Product", StringComparison.Ordinal) != -1)
            {
                return "products";
            }

            if (typeName.IndexOf("PriceCondition", StringComparison.Ordinal) != -1)
            {
                return "price-conditions";
            }

            if (typeName.IndexOf("SupplierPrice", StringComparison.Ordinal) != -1)
            {
                return "supplier-prices";
            }
            
            if (typeName.IndexOf("SupplierCurrencyRate", StringComparison.Ordinal) != -1)
            {
                return "supplier-currency-rates";
            }

            if (typeName.IndexOf("SellingPrice", StringComparison.Ordinal) != -1)
            {
                return "selling-prices";
            }

            if (typeName.IndexOf("Employee", StringComparison.Ordinal) != -1)
            {
                return "employees";
            }

            return typeName;
        }
    }
}
