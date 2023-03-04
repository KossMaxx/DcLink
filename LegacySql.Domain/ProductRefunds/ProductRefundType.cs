namespace LegacySql.Domain.ProductRefunds
{
    public enum ProductRefundType
    {
        PurchaseProducts = 0,
        ClientOrder = 1,
        Return = 3,
        Employee = 4,
        AdditionalPurchaseProducts = 5,
        CustomOrder = 6,
        PurchaseWithProductsCreation = 7,
        PurchaseWithoutReport = 8,
        Hidden = 9,
    }
}