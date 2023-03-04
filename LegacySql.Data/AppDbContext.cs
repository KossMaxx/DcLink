using LegacySql.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace LegacySql.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() 
        {
            SetSwitch();
        }

        public AppDbContext(DbContextOptions options)
            : base(options) 
        { 
            SetSwitch(); 
        }

        private void SetSwitch()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
        public DbSet<ProductMapEF> ProductMaps { get; set; }
        public DbSet<ProductTypeMapEF> ProductTypeMaps { get; set; }
        public DbSet<ClientMapEF> ClientMaps { get; set; }
        public DbSet<ClientOrderMapEF> ClientOrderMaps { get; set; }
        public DbSet<LastChangedDateEF> LastChangedDates { get; set; }
        public DbSet<ProductTypeCategoryGroupMapEF> ProductTypeCategoryGroupMaps { get; set; }
        public DbSet<ProductTypeCategoryMapEF> ProductTypeCategoryMaps { get; set; }
        public DbSet<ProductTypeCategoryParameterMapEF> ProductTypeCategoryParameterMaps { get; set; }
        public DbSet<EmployeeMapEF> EmployeeMaps { get; set; }
        public DbSet<PhysicalPersonMapEF> PhysicalPersonMaps { get; set; }
        public DbSet<RelatedProductMapEF> RelatedProductMaps { get; set; }
        public DbSet<CashboxMapEF> CashboxMaps { get; set; }
        public DbSet<WarehouseMapEF> WarehouseMaps { get; set; }
        public DbSet<PurchaseMapEF> PurchaseMaps { get; set; }
        public DbSet<RejectMapEF> RejectMaps { get; set; }
        public DbSet<ProductRefundMapEF> ProductRefundMaps { get; set; }
        public DbSet<PriceConditionMapEF> PriceConditionMaps { get; set; }
        public DbSet<ProductPriceConditionMapEF> ProductPriceConditionMaps { get; set; }
        public DbSet<NotFullMappedEF> NotFullMapped { get; set; }
        public DbSet<ExecutingJobEF> ExecutingJobs { get; set; }
        public DbSet<ErpNotFullMappedEF> ErpNotFullMapped { get; set; }
        public DbSet<DeliveryMapEF> DeliveryMaps { get; set; }
        public DbSet<BankPaymentMapEF> BankPaymentMaps { get; set; }
        public DbSet<CashboxPaymentMapEF> CashboxPaymentMaps { get; set; }
        public DbSet<ManufacturerMapEF> ManufacturerMaps { get; set; }
        public DbSet<ErpChangedEF> ErpChanged { get; set; }
        public DbSet<ClassMapEF> ClassMaps { get; set; }
        public DbSet<MarketSegmentMapEF> MarketSegmentMaps { get; set; }
        public DbSet<ProductSubtypeMapEF> ProductSubtypeMaps { get; set; }
        public DbSet<ReconciliationActMapEF> ReconciliationActMaps { get; set; }
        public DbSet<DepartmentMapEF> DepartmentMaps { get; set; }
        public DbSet<PartnerProductGroupMapEF> PartnerProductGroupMaps { get; set; }
        public DbSet<ActivityTypeMapEF> ActivityTypes { get; set; }
        public DbSet<SegmentationTurnoverMapEF> SegmentationTurnoverMaps { get; set; }
        public DbSet<PaymentOrderMapEF> PaymentOrderMaps { get; set; }
        public DbSet<CashboxApplicationPaymentMapEF> CashboxApplicationPaymentMaps { get; set; }
        public DbSet<FreeDocumentMapEF> FreeDocumentMaps { get; set; }
        public DbSet<BillMapEF> BillMaps { get; set; }
        public DbSet<FirmMapEF> FirmMaps { get; set; }
        public DbSet<MovementOrderMapEF> MovementsOrderMaps { get; set; }
        public DbSet<ProductMovingMapEF> ProductMovingMaps { get; set; }
        public DbSet<IncomingBillMapEF> IncomingBilsMaps { get; set; }
        public DbSet<WaybillMapEF> WaybillMaps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            AddBaseMapModelIndexes<ProductMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductTypeMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ClientMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductTypeCategoryMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductTypeCategoryParameterMapEF>(modelBuilder);
            AddBaseMapModelIndexes<EmployeeMapEF>(modelBuilder);
            AddBaseMapModelIndexes<PhysicalPersonMapEF>(modelBuilder);
            AddBaseMapModelIndexes<RelatedProductMapEF>(modelBuilder);
            AddBaseMapModelIndexes<WarehouseMapEF>(modelBuilder);
            AddBaseMapModelIndexes<PurchaseMapEF>(modelBuilder);
            AddBaseMapModelIndexes<RejectMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductRefundMapEF>(modelBuilder);
            AddBaseMapModelIndexes<DeliveryMapEF>(modelBuilder);
            AddBaseMapModelIndexes<PriceConditionMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductPriceConditionMapEF>(modelBuilder);
            AddBaseMapModelIndexes<CashboxPaymentMapEF>(modelBuilder);
            AddBaseMapModelIndexes<MarketSegmentMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductSubtypeMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ReconciliationActMapEF>(modelBuilder);
            AddBaseMapModelIndexes<DepartmentMapEF>(modelBuilder);
            AddBaseMapModelIndexes<PartnerProductGroupMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ActivityTypeMapEF>(modelBuilder);
            AddBaseMapModelIndexes<SegmentationTurnoverMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductTypeCategoryGroupMapEF>(modelBuilder);
            AddBaseMapModelIndexes<PaymentOrderMapEF>(modelBuilder);
            AddBaseMapModelIndexes<CashboxApplicationPaymentMapEF>(modelBuilder);
            AddBaseMapModelIndexes<FreeDocumentMapEF>(modelBuilder);
            AddBaseMapModelIndexes<BillMapEF>(modelBuilder);
            AddBaseMapModelIndexes<FirmMapEF>(modelBuilder);
            AddBaseMapModelIndexes<MovementOrderMapEF>(modelBuilder);
            AddBaseMapModelIndexes<ProductMovingMapEF>(modelBuilder);
            AddBaseMapModelIndexes<IncomingBillMapEF>(modelBuilder);
            AddBaseMapModelIndexes<WaybillMapEF>(modelBuilder); 

            modelBuilder.Entity<ClientOrderMapEF>()
                .HasIndex(e => new {e.ErpGuid, e.LegacyId})
                .IsUnique();

            modelBuilder.Entity<ProductSubtypeMapEF>()
                .HasIndex(i => i.Title);

            modelBuilder.Entity<NotFullMappedEF>()
                .HasIndex(i => new {i.InnerId, i.Type}).IsUnique();

            modelBuilder.Entity<NotFullMappedEF>()
                .HasIndex(i => i.Type);

            modelBuilder.Entity<ExecutingJobEF>()
                .HasIndex(i => i.JobType).IsUnique();

            modelBuilder.Entity<ErpNotFullMappedEF>()
                .HasIndex(e => new {e.ErpId, e.Type})
                .IsUnique();

            modelBuilder.Entity<ManufacturerMapEF>()
                .HasIndex(e => e.ErpGuid)
                .IsUnique();

            modelBuilder.Entity<ManufacturerMapEF>()
                .HasIndex(e => e.LegacyId)
                .IsUnique();

            modelBuilder.Entity<ErpChangedEF>()
                .HasIndex(e => new {e.LegacyId, e.Type})
                .IsUnique();

            modelBuilder.Entity<ClassMapEF>()
                .HasIndex(e => e.ErpGuid)
                .IsUnique();

            modelBuilder.Entity<ClassMapEF>()
                .HasIndex(e => e.LegacyTitle)
                .IsUnique();
            modelBuilder.Entity<CashboxMapEF>()
                .HasIndex(e => new { e.ErpGuid, e.LegacyId })
                .IsUnique();
        }

        private void AddBaseMapModelIndexes<T>(ModelBuilder modelBuilder)
            where T : BaseMapModel
        {
            modelBuilder.Entity<T>()
                .HasIndex(u => u.LegacyId)
                .IsUnique();
            modelBuilder.Entity<T>()
                .HasIndex(u => u.ErpGuid)
                .IsUnique();
        }
    }
}