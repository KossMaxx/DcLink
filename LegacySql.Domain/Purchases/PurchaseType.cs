namespace LegacySql.Domain.Purchases
{
    public enum PurchaseType
    {
        PurchaseProducts = 0,
        Return = 3,
        Employee = 4,
        AdditionalPurchaseProducts = 5,
        CustomOrder = 6,
        PurchaseWithProductsCreation = 7,
        PurchaseWithoutReport = 8,
        Hidden = 9,
    }
}