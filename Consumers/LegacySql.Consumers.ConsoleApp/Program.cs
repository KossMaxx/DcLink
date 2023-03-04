using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using GreenPipes;
using LegacySql.Consumers.Commands.BankPayments;
using LegacySql.Consumers.Commands.Bonuses;
using LegacySql.Consumers.Commands.Cashboxes;
using LegacySql.Consumers.Commands.Classes;
using LegacySql.Consumers.Commands.ClientOrders;
using LegacySql.Consumers.Commands.Clients;
using LegacySql.Consumers.Commands.Clients.AddClientMap;
using LegacySql.Consumers.Commands.Departments;
using LegacySql.Consumers.Commands.ErpNotFullMappings.ResaveErpNotFullMappings;
using LegacySql.Consumers.Commands.FreeDocuments;
using LegacySql.Consumers.Commands.Manufacturers;
using LegacySql.Consumers.Commands.MovementOrders;
using LegacySql.Consumers.Commands.Penalties;
using LegacySql.Consumers.Commands.PriceConditions;
using LegacySql.Consumers.Commands.ProductMovings;
using LegacySql.Consumers.Commands.ProductRefunds;
using LegacySql.Consumers.Commands.Products;
using LegacySql.Consumers.Commands.ProductSubtypes;
using LegacySql.Consumers.Commands.ProductTypeCategoryGroups;
using LegacySql.Consumers.Commands.ProductTypes;
using LegacySql.Consumers.Commands.Purchases;
using LegacySql.Consumers.Commands.Rejects;
using LegacySql.Consumers.Commands.SellingPrices;
using LegacySql.Consumers.Commands.SupplierCurrencyRates;
using LegacySql.Consumers.Commands.WarehouseStocks;
using LegacySql.Consumers.Commands.Waybills;
using LegacySql.Consumers.ConsoleApp.Consumers.AddMappings;
using LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp;
using LegacySql.Consumers.ConsoleApp.Extensions;
using LegacySql.Consumers.ConsoleApp.Infrastructure.Logger;
using LegacySql.Data;
using LegacySql.Data.Repositories;
using LegacySql.Domain.ActivityTypes;
using LegacySql.Domain.BankPayments;
using LegacySql.Domain.Bills;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.Classes;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Deliveries;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Employees;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Firms;
using LegacySql.Domain.FreeDocuments;
using LegacySql.Domain.IncomingBills;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.MarketSegments;
using LegacySql.Domain.MovementOrders;
using LegacySql.Domain.PartnerProductGroups;
using LegacySql.Domain.PhysicalPersons;
using LegacySql.Domain.PriceConditions;
using LegacySql.Domain.ProductMoving;
using LegacySql.Domain.ProductPriceConditions;
using LegacySql.Domain.ProductRefunds;
using LegacySql.Domain.Products;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.ProductTypeCategories;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Domain.ProductTypeCategoryParameters;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.ReconciliationActs;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.RelatedProducts;
using LegacySql.Domain.SegmentationTurnovers;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierCurrencyRates;
using LegacySql.Domain.Warehouses;
using LegacySql.Domain.Waybills;
using LegacySql.Legacy.Data;
using LegacySql.Legacy.Data.Clients;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using LegacySql.Legacy.Data.Firms;
using LegacySql.Legacy.Data.ProductMovings;
using LegacySql.Legacy.Data.Repositories;
using LegacySql.Legacy.Data.Repositories.ConsumerCommandContracts;
using LegacySql.Legacy.Data.Stores;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using MediatR;
using MessageBus.ActivityTypes.Import.Add;
using MessageBus.BankPayments.Import.Add;
using MessageBus.Bills.Import.Add;
using MessageBus.Bonuses.Import.Add;
using MessageBus.Cashboxes.Import.Add;
using MessageBus.Classes.Import.Add;
using MessageBus.ClientOrder.Import.Add;
using MessageBus.ClientOrder.Import.Delete;
using MessageBus.Clients.Import.Add;
using MessageBus.Deliveries.Export.Add;
using MessageBus.Deliveries.Import.Add;
using MessageBus.Departments.Import.Add;
using MessageBus.Employees.Import.Add;
using MessageBus.Firms.Import.Add;
using MessageBus.FreeDocuments.Import.Add;
using MessageBus.IncomingBills.Import.Add;
using MessageBus.Manufacturer.Import.Add;
using MessageBus.MarketSegments.Import.Add;
using MessageBus.MovementOrders.Import.Add;
using MessageBus.PartnerProductGroups.Import.Add;
using MessageBus.Penalties.Import.Add;
using MessageBus.PhysicalPersons.Import.Add;
using MessageBus.PriceConditions.Import.Add;
using MessageBus.ProductMovings.Import.Add;
using MessageBus.ProductPriceConditions.Import.Add;
using MessageBus.ProductRefunds.Import.Add;
using MessageBus.Products.Import.Add;
using MessageBus.ProductSubtypes.Import.Add;
using MessageBus.ProductTypeCategoryGroups.Import.Add;
using MessageBus.ProductTypes.Import.Add;
using MessageBus.Purchases.Import.Add;
using MessageBus.Rates.Import.Change;
using MessageBus.ReconciliationActs.Import;
using MessageBus.ReconciliationActs.Import.Add;
using MessageBus.Rejects.Import.Add;
using MessageBus.SegmentationTurnovers.Import.Add;
using MessageBus.SellingPrices.Import.Add;
using MessageBus.SupplierCurrencyRates.Import.Add;
using MessageBus.Warehouses.Import.Add;
using MessageBus.WarehouseStocks.Import.Add;
using MessageBus.Waybills.Import.Add;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace LegacySql.Consumers.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            var config = Configure();

            ConfigureLogging(services);

            #region Data

            var appConnectionString = config.ConnectionStrings.AppDbContext;
            var appOptionsBuilder = new DbContextOptionsBuilder();
            appOptionsBuilder.UseNpgsql(appConnectionString);
            services.AddTransient(ctx => new AppDbContext(appOptionsBuilder.Options));

            var legacyConnectionString = config.ConnectionStrings.LegacyDbContext;
            var legacyOptionsBuilder = new DbContextOptionsBuilder();
            legacyOptionsBuilder.UseSqlServer(legacyConnectionString,
                sqlServerOptions => sqlServerOptions.CommandTimeout(300));
            services.AddTransient(ctx => new LegacyDbContext(legacyOptionsBuilder.Options));

            services.AddScoped<IDbConnection>(ctx => new SqlConnection(legacyConnectionString));
            services.AddTransient(ctx => new LegacyDbConnection(legacyConnectionString));
            services.AddTransient(ctx => new AppDbConnection(appConnectionString));

            #endregion

            #region DependencyInjection

            services.AddTransient(typeof(IProductMapRepository), typeof(ProductMapRepository));
            services.AddTransient(typeof(IClientMapRepository), typeof(ClientMapRepository));
            services.AddTransient(typeof(IProductTypeMapRepository), typeof(ProductTypeMapRepository));
            services.AddTransient(typeof(IClientOrderMapRepository), typeof(ClientOrderMapRepository));
            services.AddTransient(typeof(IProductTypeCategoryGroupMapRepository), typeof(ProductTypeCategoryGroupMapRepository));
            services.AddTransient(typeof(IProductTypeCategoryMapRepository), typeof(ProductTypeCategoryMapRepository));
            services.AddTransient(typeof(IProductTypeCategoryParameterMapRepository), typeof(ProductTypeCategoryParameterMapRepository));
            services.AddTransient(typeof(IEmployeeMapRepository), typeof(EmployeeMapRepository));
            services.AddTransient(typeof(IPhysicalPersonMapRepository), typeof(PhysicalPersonMapRepository));
            services.AddTransient(typeof(IRelatedProductMapRepository), typeof(RelatedProductMapRepository));
            services.AddTransient(typeof(ICashboxMapRepository), typeof(CashboxMapRepository));
            services.AddTransient(typeof(IWarehouseMapRepository), typeof(WarehouseMapRepository));
            services.AddTransient(typeof(IPurchaseMapRepository), typeof(PurchaseMapRepository));
            services.AddTransient(typeof(IErpNotFullMappedRepository), typeof(ErpNotFullMappedRepository));
            services.AddTransient(typeof(IRejectMapRepository), typeof(RejectMapRepository));
            services.AddTransient(typeof(IProductRefundMapRepository), typeof(ProductRefundMapRepository));
            services.AddTransient(typeof(IDeliveryMapRepository), typeof(DeliveryMapRepository));
            services.AddTransient(typeof(IPriceConditionMapRepository), typeof(PriceConditionMapRepository));
            services.AddTransient(typeof(IProductPriceConditionMapRepository), typeof(ProductPriceConditionMapRepository));
            services.AddTransient(typeof(IBankPaymentMapRepository), typeof(BankPaymentMapRepository));
            services.AddTransient(typeof(ICashboxPaymentMapRepository), typeof(CashboxPaymentMapRepository));
            services.AddTransient(typeof(IProductMappingResolver), typeof(ProductMappingResolver));
            services.AddTransient(typeof(ILegacyManufacturerRepository), typeof(ManufacturerRepository));
            services.AddTransient(typeof(IManufacturerMapRepository), typeof(ManufacturerMapRepository));
            services.AddTransient(typeof(IErpChangedRepository), typeof(ErpChangedRepository));
            services.AddTransient(typeof(IClassMapRepository), typeof(ClassMapRepository));
            services.AddTransient(typeof(ILegacyClassRepository), typeof(ClassRepository));
            services.AddTransient(typeof(ILegacyMarketSegmentRepository), typeof(MarketSegmentRepository));
            services.AddTransient(typeof(IMarketSegmentMapRepository), typeof(MarketSegmentMapRepository));
            services.AddTransient(typeof(IProductSubtypeMapRepository), typeof(ProductSubtypeMapRepository));
            services.AddTransient(typeof(ILegacyProductSubtypeRepository), typeof(ProductSubtypeRepository));
            services.AddTransient<ErpProductSaver>();
            services.AddTransient(typeof(ILegacySupplierCurrencyRateRepository), typeof(SupplierCurrencyRateRepository));
            services.AddTransient(typeof(IReconciliationActMapRepository), typeof(ReconciliationActMapRepository));
            services.AddTransient<ErpSupplierCurrencyRateSaver>();
            services.AddTransient<ErpPurchaseSaver>();
            services.AddTransient<ErpClientOrderSaver>();
            services.AddTransient<ErpProductRefundSaver>();
            services.AddTransient<ErpSellingPriceSaver>();
            services.AddTransient<ErpClientSaver>();
            services.AddTransient<ErpClientOrderSerialNumbersSaver>();
            services.AddTransient<ErpClientOrderDeliverySaver>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatorLoggingBehaviour<,>));
            services.AddTransient<ErpBonusSaver>();
            services.AddTransient<ErpPenaltySaver>();
            services.AddTransient<ErpRejectSaver>();
            services.AddTransient<ErpWarehouseStockSaver>();
            services.AddTransient<ErpRejectReplacementCostSaver>();
            services.AddTransient<ErpBankPaymentSaver>();
            services.AddTransient<ErpCashboxPaymentSaver>();
            services.AddTransient<ErpPriceConditionSaver>();
            services.AddTransient<ErpProductSubtypeSaver>();
            services.AddTransient<ErpClassSaver>();
            services.AddTransient<ErpProductTypeSaver>();
            services.AddTransient<ErpProductTypeCategoryGroupSaver>();
            services.AddTransient<ErpClientFirmSaver>();
            services.AddTransient<PartnerSaver>();
            services.AddTransient(typeof(IDepartmentMapRepository), typeof(DepartmentMapRepository));
            services.AddTransient<ErpDepartmentSaver>();
            services.AddTransient<ErpManufacturerSaver>();
            services.AddTransient<IPartnerStore, PartnerStore>();
            services.AddTransient<IProductStore, ProductStore>();
            services.AddTransient(typeof(IPartnerProductGroupsMapRepository), typeof(PartnerProductGroupsMapRepository));
            services.AddTransient<IPartnerProductGroupsStore, PartnerProductGroupsStore>();
            services.AddTransient(typeof(IActivityTypesMapRepository), typeof(ActivityTypesMapRepository));
            services.AddTransient<IActivityTypeStore, ActivityTypeStore>();
            services.AddTransient(typeof(ISegmentationTurnoversMapRepository), typeof(SegmentationTurnoversMapRepository));
            services.AddTransient<ISegmentationTurnoverStore, SegmentationTurnoverStore>();
            services.AddTransient<IPaymentOrderMapRepository, PaymentOrderMapRepository>();
            services.AddTransient<ErpPaymentOrderSaver>();
            services.AddTransient(typeof(ICashboxApplicationPaymentMapRepository), typeof(CashboxApplicationPaymentMapRepository));
            services.AddTransient<ErpFreeDocumentSaver>();
            services.AddTransient<IFreeDocumentMapRepository, FreeDocumentMapRepository>();
            services.AddTransient<IBillMapRepository, BillMapRepository>();
            services.AddTransient<IFirmMapRepository, FirmMapRepository>();
            services.AddTransient<IMovementOrderMapRepository, MovementOrderMapRepository>();
            services.AddTransient<ErpMovementOrderSaver>();
            services.AddTransient(typeof(IProductMovingMapRepository), typeof(ProductMovingMapRepository));
            services.AddTransient<ErpProductMovingSaver>();
            services.AddTransient<ILegacyProductMovingRepository, ProductMovingRepository>();
            services.AddTransient(typeof(ILegacyClientRepository), typeof(ClientRepository));
            services.AddTransient<ILegacyFirmRepository>(
                ctx => new FirmRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    500));
            services.AddTransient<IIncomingBillMapRepository, IncomingBillMapRepository>();
            services.AddTransient<IWaybillMapRepository, WaybillMapRepository>();
            services.AddTransient<ErpWaybillSaver>();
            #endregion

            services.AddMediatR(Assembly.GetAssembly(typeof(AddClientMapCommand)));
            services.AddMediatR(Assembly.GetAssembly(typeof(AddClientMapCommandHandler)));

            services.AddMassTransit(mtConfig =>
            {
                mtConfig.AddConsumer<ProductAddConsumer>();
                mtConfig.AddConsumer<ProductTypeAddConsumer>();
                mtConfig.AddConsumer<ClientAddConsumer>();
                mtConfig.AddConsumer<ClientOrderAddConsumer>();
                mtConfig.AddConsumer<ProductTypeCategoryGroupAddConsumer>();
                mtConfig.AddConsumer<ProductTypeCategoryAddConsumer>();
                mtConfig.AddConsumer<ProductTypeCategoryParameterAddConsumer>();
                mtConfig.AddConsumer<EmployeeAddConsumer>();
                mtConfig.AddConsumer<PhysicalPersonAddConsumer>();
                mtConfig.AddConsumer<PurchaseAddConsumer>();
                mtConfig.AddConsumer<ProductTypeCategoryGroupErpSaveConsumer>();
                mtConfig.AddConsumer<ProductTypeErpSaveConsumer>();
                mtConfig.AddConsumer<RejectAddConsumer>();
                mtConfig.AddConsumer<ProductRefundAddConsumer>();
                mtConfig.AddConsumer<ProductErpSaveConsumer>();
                mtConfig.AddConsumer<ClientOrderErpSaveConsumer>();
                mtConfig.AddConsumer<RateErpSaveConsumer>();
                mtConfig.AddConsumer<ClientOrderDeliveryErpSaveConsumer>();
                mtConfig.AddConsumer<ClientOrderSerialNumbersErpSaveConsumer>();
                mtConfig.AddConsumer<SellingPriceErpSaveConsumer>();
                mtConfig.AddConsumer<PriceConditionErpSaveConsumer>();
                mtConfig.AddConsumer<ProductPriceConditionErpSaveConsumer>();
                mtConfig.AddConsumer<BankPaymentErpSaveConsumer>();
                mtConfig.AddConsumer<ClientErpSaveConsumer>();
                mtConfig.AddConsumer<CashboxPaymentErpSaveConsumer>();
                mtConfig.AddConsumer<WarehouseStockErpSaveConsumer>();
                mtConfig.AddConsumer<ManufacturerAddConsumer>();
                mtConfig.AddConsumer<ManufacturerErpSaveConsumer>();
                mtConfig.AddConsumer<ClassAddConsumer>();
                mtConfig.AddConsumer<ClassErpSaveConsumer>();
                mtConfig.AddConsumer<EmployeeErpSaveConsumer>();
                mtConfig.AddConsumer<MarketSegmentAddConsumer>();
                mtConfig.AddConsumer<ProductSubtypeAddConsumer>();
                mtConfig.AddConsumer<ProductSubtypeErpSaveConsumer>();
                mtConfig.AddConsumer<PenaltyErpSaveConsumer>();
                mtConfig.AddConsumer<BonusErpSaveConsumer>();
                mtConfig.AddConsumer<WarehouseErpSaveConsumer>();
                mtConfig.AddConsumer<MarketSegmentErpSaveConsumer>();
                mtConfig.AddConsumer<ReconciliationActAddConsumer>();
                mtConfig.AddConsumer<ReconciliationActErpSaveConsumer>();
                mtConfig.AddConsumer<SupplierCurrencyRateErpSaveConsumer>();
                mtConfig.AddConsumer<PurchaseErpSaveConsumer>();
                mtConfig.AddConsumer<ProductRefundErpSaveConsumer>();
                mtConfig.AddConsumer<ClientOrderErpDeleteConsumer>();
                mtConfig.AddConsumer<RejectErpSaveConsumer>();
                mtConfig.AddConsumer<RejectReplacementCostErpSaveConsumer>();
                mtConfig.AddConsumer<ClientFirmErpSaveConsumer>();
                mtConfig.AddConsumer<PartnerConsumer>();
                mtConfig.AddConsumer<DepartmentAddConsumer>();
                mtConfig.AddConsumer<DepartmentErpSaveConsumer>();
                mtConfig.AddConsumer<PartnerProductGroupAddConsumer>();
                mtConfig.AddConsumer<PartnerProductGroupErpSaveConsumer>();
                mtConfig.AddConsumer<ActivityTypeAddConsumer>();
                mtConfig.AddConsumer<ActivityTypeErpSaveConsumer>();
                mtConfig.AddConsumer<SegmentationTurnoverAddConsumer>();
                mtConfig.AddConsumer<SegmentationTurnoverErpSaveConsumer>();
                mtConfig.AddConsumer<CashboxPaymentsAddConsumer>();
                mtConfig.AddConsumer<PaymentOrderErpSaveConsumer>();
                mtConfig.AddConsumer<CashboxApplicationPaymentsAddConsumer>();
                mtConfig.AddConsumer<FreeDocumentErpSaveConsumer>();
                mtConfig.AddConsumer<BillAddConsumer>();
                mtConfig.AddConsumer<FirmAddConsumer>();
                mtConfig.AddConsumer<MovementOrderErpSaveConsumer>();
                mtConfig.AddConsumer<ProductMovingAddConsumer>();
                mtConfig.AddConsumer<ProductMovingErpSaveConsumer>();
                mtConfig.AddConsumer<IncomingBillAddConsumer>();
                mtConfig.AddConsumer<WaybillErpSaveConsumer>();
                mtConfig.AddConsumer<DeliveryAddConsumer>();

                mtConfig.AddBus(context => Bus.Factory.CreateUsingRabbitMq(busConfig =>
                {
                    busConfig.Host(config.RabbitMq.Host, config.RabbitMq.Port, "/", hostConfig =>
                    {
                        hostConfig.Username(config.RabbitMq.Username);
                        hostConfig.Password(config.RabbitMq.Password);
                    });

                    busConfig.UseCustomLogger();

                    busConfig.ReceiveEndpoint("erp_products-add",
                        e => ConfigureEndpoint<AddedProductMessage, ProductAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-types-add",
                        e => ConfigureEndpoint<AddedProductTypeMessage, ProductTypeAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-type-category-groups-add",
                        e => ConfigureEndpoint<AddedProductTypeCategoryGroupMessage, ProductTypeCategoryGroupAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_clients-add",
                        e => ConfigureEndpoint<AddedClientMessage, ClientAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_client-orders-add",
                        e => ConfigureEndpoint<AddedClientOrderMessage, ClientOrderAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-type-category-add",
                        e => ConfigureEndpoint<AddedProductTypeCategoryMessage, ProductTypeCategoryAddConsumer>(e,
                            context));
                    busConfig.ReceiveEndpoint("erp_product-type-category-parameters-add",
                        e => ConfigureEndpoint<AddedProductTypeCategoryParameterMessage,
                            ProductTypeCategoryParameterAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_employees-add",
                        e => ConfigureEndpoint<AddedEmployeeMessage, EmployeeAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_physical-persons-add",
                        e => ConfigureEndpoint<AddedPhysicalPersonMessage, PhysicalPersonAddConsumer>(e,
                            context));
                    busConfig.ReceiveEndpoint("erp_purchases-add",
                        e => ConfigureEndpoint<AddedPurchaseMessage, PurchaseAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_productRefunds-add",
                        e => ConfigureEndpoint<AddedProductRefundMessage, ProductRefundAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-types-publish",
                        e => ConfigureEndpoint<PublishErpProductTypeMessage, ProductTypeErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-type-category-group-publish",
                        e => ConfigureEndpoint<PublishErpProductTypeCategoryGroupMessage, ProductTypeCategoryGroupErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_rejects-add",
                        e => ConfigureEndpoint<AddedRejectMessage, RejectAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-publish",
                        e => ConfigureEndpoint<PublishErpProductMessage, ProductErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_client-order-publish",
                        e => ConfigureEndpoint<PublishErpClientOrderMessage, ClientOrderErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_rate-publish",
                        e => ConfigureEndpoint<ErpChangeRateMessage, RateErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_client-order-delivery-publish",
                        e => ConfigureEndpoint<PublishErpClientOrderDeliveryMessage, ClientOrderDeliveryErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_client-order-serial-numbers-publish",
                        e => ConfigureEndpoint<PublishErpClientOrderSerialNumbersMessage, ClientOrderSerialNumbersErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_selling-price-publish",
                        e => ConfigureEndpoint<PublishErpSellingPriceMessage, SellingPriceErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_price-condition-publish",
                        e => ConfigureEndpoint<PublishErpPriceConditionMessage, PriceConditionErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-price-condition-publish",
                        e => ConfigureEndpoint<PublishErpProductPriceConditionMessage, ProductPriceConditionErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_bank-payment-publish",
                        e => ConfigureEndpoint<PublishErpBankPaymentMessage, BankPaymentErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_client-publish",
                        e => ConfigureEndpoint<PublishErpClientMessage, ClientErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_cashbox-payment-publish",
                        e => ConfigureEndpoint<PublishErpCashboxPaymentMessage, CashboxPaymentErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_warehouse-stock-publish",
                        e => ConfigureEndpoint<PublishErpWarehouseStockMessage, WarehouseStockErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_manufacturers-add",
                        e => ConfigureEndpoint<AddedManufacturerMessage, ManufacturerAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_manufacturer-publish",
                        e => ConfigureEndpoint<PublishErpManufacturerMessage, ManufacturerErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_class-add",
                        e => ConfigureEndpoint<AddedClassMessage, ClassAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_class-publish",
                        e => ConfigureEndpoint<PublishErpClassMessage, ClassErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_employee-publish",
                        e => ConfigureEndpoint<PublishErpEmployeeMessage, EmployeeErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_market-segments-add",
                        e => ConfigureEndpoint<AddedMarketSegmentMessage, MarketSegmentAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-subtypes-add",
                        e => ConfigureEndpoint<AddedProductSubtypeMessage, ProductSubtypeAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-subtype-publish",
                        e => ConfigureEndpoint<PublishErpProductSubtypeMessage, ProductSubtypeErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_bonus-publish",
                        e => ConfigureEndpoint<PublishErpBonusMessage, BonusErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_penalty-publish",
                        e => ConfigureEndpoint<PublishErpPenaltyMessage, PenaltyErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_warehouse-publish",
                        e => ConfigureEndpoint<PublishErpWarehouseMessage, WarehouseErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_market-segment-publish",
                        e => ConfigureEndpoint<PublishErpMarketSegmentMessage, MarketSegmentErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_reconciliation-acts-add",
                        e => ConfigureEndpoint<AddedReconciliationActMessage, ReconciliationActAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_reconciliation-act-publish",
                        e => ConfigureEndpoint<PublishErpReconciliationActMessage, ReconciliationActErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_supplier-currency-rate-publish",
                        e => ConfigureEndpoint<PublishErpSupplierCurrencyRateMessage, SupplierCurrencyRateErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_purchase-publish",
                        e => ConfigureEndpoint<PublishErpPurchaseMessage, PurchaseErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-refund-publish",
                        e => ConfigureEndpoint<PublishErpProductRefundMessage, ProductRefundErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_client-order-delete",
                        e => ConfigureEndpoint<DeleteErpClientOrderMessage, ClientOrderErpDeleteConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_reject-publish",
                        e => ConfigureEndpoint<PublishErpRejectMessage, RejectErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_reject-replacement-cost-publish",
                        e => ConfigureEndpoint<PublishErpRejectReplacementCostMessage, RejectReplacementCostErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_client-firm-publish",
                        e => ConfigureEndpoint<PublishErpClientFirmMessage, ClientFirmErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_partner-publish",
                        e => ConfigureEndpoint<PublishErpPartnerMessage, PartnerConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_departments-add",
                        e => ConfigureEndpoint<AddedDepartmentMessage, DepartmentAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_department-publish",
                        e => ConfigureEndpoint<PublishErpDepartmentMessage, DepartmentErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_partner-product-group-add",
                        e => ConfigureEndpoint<AddedPartnerProductGroupMessage, PartnerProductGroupAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_partner-product-group-publish",
                        e => ConfigureEndpoint<PublishErpPartnerProductGroupMessage, PartnerProductGroupErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_activity-type-add",
                        e => ConfigureEndpoint<AddedActivityTypeMessage, ActivityTypeAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_activity-type-publish",
                        e => ConfigureEndpoint<PublishErpActivityTypeMessage, ActivityTypeErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_segmentation-turnover-add",
                        e => ConfigureEndpoint<AddedSegmentationTurnoverMessage, SegmentationTurnoverAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_segmentation-turnover-publish",
                        e => ConfigureEndpoint<PublishErpSegmentationTurnoverMessage, SegmentationTurnoverErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_cashbox-payment-add",
                        e => ConfigureEndpoint<AddedCashboxPaymentMessage, CashboxPaymentsAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_cashbox-application-payment-add",
                        e => ConfigureEndpoint<AddedCahsboxApplicationPaymentMessage, CashboxApplicationPaymentsAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_payment-order-publish",
                        e => ConfigureEndpoint<PublishErpPaymentOrderMessage, PaymentOrderErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_free-document-publish",
                        e => ConfigureEndpoint<PublishErpFreeDocumentMessage, FreeDocumentErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_bill-add",
                        e => ConfigureEndpoint<AddedBillMessage, BillAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_firm-add",
                        e => ConfigureEndpoint<AddedFirmMessage, FirmAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_movement-order-add",
                        e => ConfigureEndpoint<PublishErpMovementOrderMessage, MovementOrderErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-movings-add",
                        e => ConfigureEndpoint<AddedProductMovingMessage, ProductMovingAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_product-movings-publish",
                        e => ConfigureEndpoint<PublishErpProductMovingMessage, ProductMovingErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_incoming-bill-add",
                        e => ConfigureEndpoint<AddedIncomingBillMessage, IncomingBillAddConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_waybills-publish",
                        e => ConfigureEndpoint<PublishErpWaybillMessage, WaybillErpSaveConsumer>(e, context));
                    busConfig.ReceiveEndpoint("erp_deliveries-add",
                        e => ConfigureEndpoint<AddedDeliveryMessage, DeliveryAddConsumer>(e, context));
                }));
            });

            var provider = services.BuildServiceProvider();
            var bus = provider.GetRequiredService<IBusControl>();
            var busHandle = TaskUtil.Await(() => bus.StartAsync());

            StartErpNotFullMappingsCheck(provider);

            try
            {
                Console.WriteLine("Application started");
                await Task.Run(() => Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("Application stoped");
                busHandle.Stop();
            }
        }

        private static void ConfigureEndpoint<TMessage, TConsumer>(IRabbitMqReceiveEndpointConfigurator e, IBusRegistrationContext context)
            where TMessage : class
            where TConsumer : class, IConsumer
        {
            e.Bind<TMessage>();
            e.Consumer<TConsumer>(() =>
            {
                var serviceProvider = context.CreateScope().ServiceProvider;
                return serviceProvider.GetRequiredService<TConsumer>();
            },
                c =>
                {
                    c.UseMessageRetry(r => { r.Intervals(1000, 2000, 3000, 4000, 5000); });
                });
        }

        private static LegacySqlConfig Configure()
        {
            var config = new LegacySqlConfig
            {
                ConnectionStrings = new LegacySqlConfig.ConnectionStringsConfig
                {
                    //AppDbContext = "Server=192.168.32.50; User Id=postgres; Password=dAXYbtZ7M6ZM; Database=legacy_sql; CommandTimeout=0; Timeout=1024;",
                    AppDbContext = Environment.GetEnvironmentVariable("ConnectionStrings__AppDbContext"),
                    //AppDbContext = "Server=localhost; User Id=postgres; Password=pgpwd4habr; Database=legacy_sql; CommandTimeout=0; Timeout=1024;",
                    LegacyDbContext = Environment.GetEnvironmentVariable("ConnectionStrings__LegacyDbContext")
                    //LegacyDbContext = "Server=192.168.3.227;Database=skl2008_tqm; User Id=tqm; Password=drPa5bjy; Connection Timeout=300"
                },
                RabbitMq = new LegacySqlConfig.RabbitMqConfig
                {
                    HostAddress = Environment.GetEnvironmentVariable("RabbitMq__HostAddress"),
                    //HostAddress = "rabbitmq://localhost",
                    Username = Environment.GetEnvironmentVariable("RabbitMq__Username"),
                    //Username = "guest",
                    Password = Environment.GetEnvironmentVariable("RabbitMq__Password")
                    //Password = "guest"
                }
            };

            new LegacySqlConfigValidator().ValidateAndThrow(config);

            return config;
        }

        private static void StartErpNotFullMappingsCheck(IServiceProvider provider)
        {
            var timer = new System.Timers.Timer(60000);
            timer.Elapsed += async (e, arg) =>
            {
                timer.Stop();
                var scope = provider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new ResaveErpNotFullMappingsCommand());
                timer.Start();
            };
            timer.Start();
        }

        private static void ConfigureLogging(ServiceCollection services)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.WithProperty("Service", "LegacySqlConsumers")
                .Enrich.FromLogContext()
                .WriteTo.Console( /*new RenderedCompactJsonFormatter()*/)
                .WriteTo.File(path: $"{AppDomain.CurrentDomain.BaseDirectory}/logs/seq_logs_.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} ::::: Properties: {Properties:j}{NewLine}{Exception}",
                    shared: true);

            var seqUrl = Environment.GetEnvironmentVariable("Seq_Url");
            if (!string.IsNullOrEmpty(seqUrl))
            {
                loggerConfiguration.WriteTo.Seq(seqUrl, eventBodyLimitBytes: 500000);
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog(logger: Log.Logger, dispose: true);
            });
        }
    }
}