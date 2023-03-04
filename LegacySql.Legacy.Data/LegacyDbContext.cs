using System.Reflection;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegacySql.Legacy.Data
{
    public partial class LegacyDbContext : DbContext
    {
        public LegacyDbContext()
        { }

        public LegacyDbContext(DbContextOptions options)
            : base(options)
        {
        }


        public virtual DbSet<ProductEF> Products { get; set; }
        public virtual DbSet<ProductPictureEF> ProductPictures { get; set; }
        public virtual DbSet<ProductVideoEF> ProductVideo { get; set; }
        public virtual DbSet<ProductDescriptionEF> ProductDescriptions { get; set; }
        public virtual DbSet<LanguageEF> Languages { get; set; }
        public virtual DbSet<CountryIsoCodeEF> CountryIsoCodes { get; set; }
        public virtual DbSet<CountryEF> Countries { get; set; }
        public virtual DbSet<ProductTypeEF> ProductTypes { get; set; }
        public virtual DbSet<ProductTypeCategoryGroupEF> ProductTypeCategoryGroups { get; set; }
        public virtual DbSet<ProductTypeCategoryEF> ProductTypeCategories { get; set; }
        public virtual DbSet<ProductTypeCategoryParameterEF> ProductTypeCategoryParameters { get; set; }
        public virtual DbSet<ClientEF> Clients { get; set; }
        public virtual DbSet<FirmEF> Firms { get; set; }
        public virtual DbSet<ClientOrderEF> ClientOrders { get; set; }
        public virtual DbSet<ClientOrderItemEF> ClientOrderItems { get; set; }
        public virtual DbSet<ProductCategoryParameterEF> ProductCategories { get; set; }
        public virtual DbSet<EmployeeEF> Employees { get; set; }
        public virtual DbSet<PhysicalPersonEF> PhysicalPersons { get; set; }
        public virtual DbSet<DepartmentEF> Departments { get; set; }
        public virtual DbSet<RelatedProductEF> RelatedProducts { get; set; }
        public virtual DbSet<ConnectedDocumentsEF> ConnectedDocuments { get; set; }
        public virtual DbSet<DeliveryEF> Deliveries { get; set; }
        public virtual DbSet<NewPostCityEF> NewPostCities { get; set; }
        public virtual DbSet<DeliveryTypeEF> DeliveryTypes { get; set; }
        public virtual DbSet<CarrierTypeEF> CarrierTypes { get; set; }
        public virtual DbSet<SoldProductSerialNumberEF> SoldProductSerialNumbers { get; set; }
        public virtual DbSet<CashboxEF> Cashboxes { get; set; }
        public virtual DbSet<WarehouseEF> Warehouses { get; set; }
        public virtual DbSet<ClientOrderArchivalEF> ClientOrdersArchival { get; set; }
        public virtual DbSet<ClientOrderItemArchivalEF> ClientOrderItemsArchival { get; set; }
        public virtual DbSet<WarehouseStockEF> WarehouseStocks { get; set; }
        public virtual DbSet<SupplierActualPriceEF> SupplierActualPrices { get; set; }
        public virtual DbSet<SellingPriceEF> SellingPrices { get; set; }
        public virtual DbSet<SupplierHistoryPriceEF> SupplierHistoryPrices { get; set; }
        public virtual DbSet<NotInStockStatusEF> NotInStockStatuses { get; set; }
        public virtual DbSet<SupplierCurrencyRateEF> SupplierCurrencyRates { get; set; }
        public virtual DbSet<PurchaseEF> Purchases { get; set; }
        public virtual DbSet<MarketplaceDeliveryEF> MarketplaceDeliveries { get; set; }
        public virtual DbSet<RejectEF> Rejects { get; set; }
        public virtual DbSet<ProductRefundEF> ProductRefunds { get; set; }
        public virtual DbSet<PriceConditionEF> PriceConditions { get; set; }
        public virtual DbSet<ProductPriceConditionEF> ProductPriceConditions { get; set; }
        public virtual DbSet<MarketSegmentEF> MarketSegments { get; set; }
        public virtual DbSet<ProductSubtypeEF> ProductSubtypes { get; set; }
        public virtual DbSet<ReconciliationActEF> ReconciliationActs { get; set; }
        public virtual DbSet<CompanyEF> Companies { get; set; }
        public virtual DbSet<RateEF> Rates { get; set; }
        public virtual DbSet<CashboxPaymentEF> CashboxPayments { get; set; }
        public virtual DbSet<CashboxApplicationPaymentEF> CashboxApplicationPayments { get; set; }
        public virtual DbSet<BillItemEF> BillItems { get; set; }
        public virtual DbSet<BillEF> Bills { get; set; }
        public virtual DbSet<IncomingBillItemEF> IncomingBillItems { get; set; }
        public virtual DbSet<IncomingBillEF> IncomingBills { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //https://github.com/dotnet/efcore/issues/13162
            modelBuilder.Entity<EmployeeEF>().HasOne<PhysicalPersonEF>().WithOne().HasForeignKey<EmployeeEF>(e => e.Id);
            modelBuilder.Entity<ProductRefundEF>().HasOne<PurchaseEF>().WithOne().HasForeignKey<ProductRefundEF>(e => e.Id);
            modelBuilder.Entity<ProductRefundItemEF>().HasOne<PurchaseItemEF>().WithOne().HasForeignKey<ProductRefundItemEF>(e => e.Id);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
