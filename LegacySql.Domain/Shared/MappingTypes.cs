using System.Text.Json.Serialization;

namespace LegacySql.Domain.Shared
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MappingTypes
    {
        Product = 1,
        ProductType,
        Client,
        ClientOrder,
        Class,
        SupplierPrice,
        ProductTypeCategory,
        ProductTypeCategoryParameter,
        SellingPrice,
        Employee,
        PhysicalPerson,
        RelatedProduct,
        ClientOrderArchival,
        WarehouseStock,
        Purchase,
        Reject,
        ProductRefund,
        PriceCondition,
        ProductPriceCondition,
        ClientOrderSerialNumbers,
        BankPayment,
        CashboxPayment,
        Manufacturer,
        MarketSegment,
        ProductSubtype,
        Penalty,
        Bonus,
        SupplierCurrencyRate,
        ReconciliationAct,
        ClientOrderDelivery,
        RejectReplacementCost,
        ClientFirm,
        Partner,
        Department,
        PaymentOrder,
        CashboxApplicationPayment,
        FreeDocument,
        Bill,
        MovementOrder,
        ProductMoving,
        IncomingBill,
        Waybill,
        Delivery
    }
}
