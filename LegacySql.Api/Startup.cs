using System.Data;
using System.Reflection;
using FluentValidation;
using LegacySql.Api.Infrastructure;
using LegacySql.Api.Infrastructure.Logger;
using LegacySql.Commands.Clients.PublishClients;
using LegacySql.Commands.ExecutingJobs.ClearExecutingJobsCommand;
using LegacySql.Commands.PriceAlgorythm.SaveErpPriceAlgorythm;
using LegacySql.Commands.Shared;
using LegacySql.Legacy.Data;
using LegacySql.Legacy.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using MassTransit.Util;
using MediatR;
using LegacySql.Data;
using LegacySql.Data.Repositories;
using LegacySql.Domain.BankPayments;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.Classes;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Countries;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Employees;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Languages;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.MarketSegments;
using LegacySql.Domain.PhysicalPersons;
using LegacySql.Domain.Pictures;
using LegacySql.Domain.PriceConditions;
using LegacySql.Domain.ProductDescriptions;
using LegacySql.Domain.ProductPriceConditions;
using LegacySql.Domain.ProductRefunds;
using LegacySql.Domain.Products;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.ProductTypeCategories;
using LegacySql.Domain.ProductTypeCategoryParameters;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.ReconciliationActs;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.RelatedProducts;
using LegacySql.Domain.SellingPrices;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierCurrencyRates;
using LegacySql.Domain.SupplierPrice;
using LegacySql.Domain.Warehouses;
using LegacySql.Domain.WarehouseStock;
using LegacySql.Queries.Products.GetStatusOfProduct;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using LegacySql.Commands;
using Sagas.Contracts;
using Sagas.SagaLogger.Serilog;
using Serilog;
using LegacySql.Legacy.Data.Clients;
using LegacySql.Legacy.Data.Products;
using LegacySql.Domain.PartnerProductGroups;
using LegacySql.Legacy.Data.PartnerProductGroups;
using LegacySql.Domain.ActivityTypes;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Legacy.Data.ActivityTypes;
using LegacySql.Domain.SegmentationTurnovers;
using LegacySql.Legacy.Data.SegmentationTurnovers;
using LegacySql.Domain.Rates;
using LegacySql.Domain.Bills;
using LegacySql.Legacy.Data.Bills;
using LegacySql.Domain.Firms;
using LegacySql.Legacy.Data.Firms;
using LegacySql.Domain.ProductMoving;
using LegacySql.Legacy.Data.ProductMovings;
using LegacySql.Domain.IncomingBills;
using LegacySql.Legacy.Data.IncomingBills;
using LegacySql.Legacy.Data.Deliveries;
using LegacySql.Domain.Deliveries;

namespace LegacySql.Api
{
    public class Startup
    {
        private LegacySqlConfig _config { get; }
        private IWebHostEnvironment _env { get; }

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            _config = CreateConfig(config);
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMediatR(typeof(Startup));

            #region Data

            var legacyConnectionString = _config.ConnectionStrings.LegacyDbContext;
            var legacyOptionsBuilder = new DbContextOptionsBuilder();

            legacyOptionsBuilder.UseSqlServer(legacyConnectionString,
                sqlServerOptions => sqlServerOptions.CommandTimeout(300));

#if DEBUG
            var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetService<ILoggerFactory>();
            legacyOptionsBuilder.UseLoggerFactory(loggerFactory);
#endif
            var appConnectionString = _config.ConnectionStrings.AppDbContext;
            var appOptionsBuilder = new DbContextOptionsBuilder();
            appOptionsBuilder.UseNpgsql(appConnectionString);

            #endregion

            #region DependencyInjection
            services.AddTransient<ISqlMessageFactory, SqlMessageFactory>();

            services.AddMediatR(Assembly.GetAssembly(typeof(PublishClientsCommandHandler)));
            services.AddMediatR(Assembly.GetAssembly(typeof(GetStatusOfProductQueryHandler)));
            services.AddScoped(ctx => new LegacyDbContext(legacyOptionsBuilder.Options));
            services.AddTransient<IDbConnection>(ctx => new SqlConnection(legacyConnectionString));
            services.AddTransient(ctx => new LegacyDbConnection(legacyConnectionString));
            services.AddTransient(ctx => new AppDbContext(appOptionsBuilder.Options));
            services.AddTransient(ctx => new AppDbConnection(appConnectionString));
            services.AddTransient(typeof(ILegacyPictureRepository), typeof(PictureRepository));
            services.AddTransient(typeof(ILegacyCountryRepository), typeof(CountryRepository));
            services.AddTransient(typeof(ILegacyLanguageRepository), typeof(LanguageRepository));
            services.AddTransient(typeof(ILegacyProductDescriptionRepository), typeof(ProductDescriptionRepository));
            services.AddTransient(typeof(ILegacyClientRepository), typeof(ClientRepository));
            services.AddTransient(typeof(IProductMapRepository), typeof(ProductMapRepository));
            services.AddTransient(typeof(IClientMapRepository), typeof(ClientMapRepository));
            services.AddTransient(typeof(ILegacyProductTypeRepository), typeof(ProductTypeRepository));
            services.AddTransient(typeof(IProductTypeMapRepository), typeof(ProductTypeMapRepository));
            services.AddTransient(typeof(ILegacyProductTypeCategoryGroupRepository), typeof(ProductTypeCategoryGroupRepository));
            services.AddTransient(typeof(IProductTypeCategoryGroupMapRepository), typeof(ProductTypeCategoryGroupMapRepository));
            services.AddTransient(typeof(IClientOrderMapRepository), typeof(ClientOrderMapRepository));
            services.AddTransient(typeof(IRejectMapRepository), typeof(RejectMapRepository));
            services.AddTransient(typeof(ILastChangedDateRepository), typeof(LastChangedDateRepository));
            services.AddTransient(typeof(INotFullMappedRepository), typeof(NotFullMappedRepository));
            services.AddTransient(typeof(ICommandsHandlerManager), typeof(CommandsHandlerManager));
            services.AddTransient(typeof(IProductTypeCategoryMapRepository), typeof(ProductTypeCategoryMapRepository));
            services.AddTransient(typeof(IProductTypeCategoryParameterMapRepository),
                typeof(ProductTypeCategoryParameterMapRepository));
            services.AddTransient(typeof(IEmployeeMapRepository), typeof(EmployeeMapRepository));
            services.AddTransient(typeof(ILegacyEmployeeRepository), typeof(EmployeeRepository));
            services.AddTransient(typeof(IPhysicalPersonMapRepository), typeof(PhysicalPersonMapRepository));
            services.AddTransient(typeof(ILegacyPhysicalPersonRepository), typeof(PhysicalPersonRepository));
            services.AddTransient(typeof(IRelatedProductMapRepository), typeof(RelatedProductMapRepository));
            services.AddTransient(typeof(ICashboxMapRepository), typeof(CashboxMapRepository));
            services.AddTransient(typeof(ILegacyCashboxRepository), typeof(CashboxRepository));
            services.AddTransient(typeof(IWarehouseMapRepository), typeof(WarehouseMapRepository));
            services.AddTransient(typeof(ILegacyWarehouseRepository), typeof(WarehouseRepository));
            services.AddTransient(typeof(ILegacyClientOrderArchivalRepository), typeof(ClientOrderArchivalRepository));
            services.AddTransient(typeof(ILegacyWarehouseStockRepository), typeof(WarehouseStockRepository));
            services.AddTransient(typeof(IPurchaseMapRepository), typeof(PurchaseMapRepository));
            services.AddTransient(typeof(ILegacySellingPriceRepository), typeof(SellingPriceRepository));
            services.AddTransient(typeof(IProductRefundMapRepository), typeof(ProductRefundMapRepository));
            services.AddTransient(typeof(ILegacyPriceConditionRepository), typeof(PriceConditionRepository));
            services.AddTransient(typeof(ILegacyProductPriceConditionRepository),
                typeof(ProductPriceConditionRepository));
            services.AddTransient(typeof(IPriceConditionMapRepository), typeof(PriceConditionMapRepository));
            services.AddTransient(typeof(IProductPriceConditionMapRepository),
                typeof(ProductPriceConditionMapRepository));
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
            services.AddTransient(typeof(IErpNotFullMappedRepository), typeof(ErpNotFullMappedRepository));
            services.AddTransient(typeof(ILegacySupplierCurrencyRateRepository),
                typeof(SupplierCurrencyRateRepository));
            services.AddTransient(typeof(IReconciliationActMapRepository), typeof(ReconciliationActMapRepository));
            services.AddTransient<ErpPriceAlgorythmSaver>();
            services.AddTransient<LoggerService>();
            services.AddTransient(typeof(ILegacyDepartmentRepository), typeof(DepartmentRepository));
            services.AddTransient(typeof(IDepartmentMapRepository), typeof(DepartmentMapRepository));
            services.AddTransient(typeof(ILegacyPartnerProductGroupsRepository), typeof(PartnerProductGroupsRepository));
            services.AddTransient(typeof(IPartnerProductGroupsMapRepository), typeof(PartnerProductGroupsMapRepository));
            services.AddTransient(typeof(IActivityTypesMapRepository), typeof(ActivityTypesMapRepository));
            services.AddTransient(typeof(ILegacyActivityTypesRepository), typeof(ActivityTypesRepository));
            services.AddTransient(typeof(ISegmentationTurnoversMapRepository), typeof(SegmentationTurnoversMapRepository));
            services.AddTransient(typeof(ILegacySegmentationTurnoversRepository), typeof(SegmentationTurnoversRepository));
            services.AddTransient(typeof(ILegacyRateRepository), typeof(RateRepository));
            services.AddTransient(typeof(ILegacyCashboxPaymentRepository), typeof(CashboxPaymentRepository));
            services.AddTransient(typeof(ILegacyCashboxApplicationPaymentRepository), typeof(CashboxApplicationPaymentRepository));
            services.AddTransient(typeof(ICashboxApplicationPaymentMapRepository), typeof(CashboxApplicationPaymentMapRepository));
            services.AddTransient(typeof(IBillMapRepository), typeof(BillMapRepository));
            services.AddTransient(typeof(IFirmMapRepository), typeof(FirmMapRepository)); 
            services.AddTransient(typeof(IProductMovingMapRepository), typeof(ProductMovingMapRepository));
            services.AddTransient(typeof(ILegacyProductMovingRepository), typeof(ProductMovingRepository));
            services.AddTransient(typeof(IIncomingBillMapRepository), typeof(IncomingBillMapRepository));
            services.AddTransient(typeof(IDeliveryMapRepository), typeof(DeliveryMapRepository));


            var notFullMappingIdPortion = int.Parse(_config.LegacySqlFilters.NotFullMappingIdPortion);

            services.AddTransient<ILegacyDeliveryRepository>(
                ctx => new DeliveryRepository(
                    new LegacyDbConnection(legacyConnectionString),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion)
                );

            services.AddTransient<ILegacyProductRepository>(
                ctx => new ProductRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    new ProductTypeRepository(new LegacyDbContext(legacyOptionsBuilder.Options),
                        new AppDbContext(appOptionsBuilder.Options)),
                    new LegacyDbConnection(legacyConnectionString))
                );

            services.AddTransient<ILegacyClientOrderRepository>(
                ctx => new ClientOrderRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion,
                    new ProductMappingResolver(new LegacyDbConnection(legacyConnectionString),
                        new AppDbContext(appOptionsBuilder.Options)),
                    new LegacyDbConnection(legacyConnectionString)));

            services.AddTransient<ILegacySupplierPriceRepository>(
                ctx => new SupplierPriceRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion,
                    new ProductMappingResolver(new LegacyDbConnection(legacyConnectionString),
                        new AppDbContext(appOptionsBuilder.Options))));

            services.AddTransient<ILegacyRelatedProductRepository>(
                ctx => new RelatedProductRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new ProductTypeRepository(new LegacyDbContext(legacyOptionsBuilder.Options),
                        new AppDbContext(appOptionsBuilder.Options)),
                    notFullMappingIdPortion, new ProductMappingResolver(
                        new LegacyDbConnection(legacyConnectionString),
                        new AppDbContext(appOptionsBuilder.Options))));

            services.AddTransient<ILegacyPurchaseRepository>(
                ctx => new PurchaseRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion, new ProductMappingResolver(
                        new LegacyDbConnection(legacyConnectionString),
                        new AppDbContext(appOptionsBuilder.Options)),
                    new LegacyDbConnection(legacyConnectionString)));

            services.AddTransient<ILegacyRejectRepository>(
                ctx => new RejectRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion, new ProductMappingResolver(
                        new LegacyDbConnection(legacyConnectionString),
                        new AppDbContext(appOptionsBuilder.Options))));

            services.AddTransient<ILegacyProductRefundRepository>(
                ctx => new ProductRefundRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion, new ProductMappingResolver(
                        new LegacyDbConnection(legacyConnectionString),
                        new AppDbContext(appOptionsBuilder.Options))));

            services.AddTransient<ILegacyReconciliationActRepository>(
                ctx => new ReconciliationActRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion));

            services.AddTransient<ILegacyBillRepository>(
                ctx => new BillRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),
                    notFullMappingIdPortion,
                    new ProductMappingResolver(new LegacyDbConnection(legacyConnectionString),
                        new AppDbContext(appOptionsBuilder.Options)),
                    new LegacyDbConnection(legacyConnectionString)));

            services.AddTransient<ILegacyFirmRepository>(
                ctx => new FirmRepository(
                    new LegacyDbContext(legacyOptionsBuilder.Options),
                    new AppDbContext(appOptionsBuilder.Options),                    
                    notFullMappingIdPortion));

            services.AddTransient<ILegacyIncomingBillRepository>(
               ctx => new IncomingBillRepository(
                   new LegacyDbContext(legacyOptionsBuilder.Options),
                   new LegacyDbConnection(legacyConnectionString),
                   new AppDbContext(appOptionsBuilder.Options),
                   new ProductMappingResolver(new LegacyDbConnection(legacyConnectionString),
                       new AppDbContext(appOptionsBuilder.Options)),
                   notFullMappingIdPortion
                   ));

            services.AddSingleton<LoggingSettingsManager>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatorLoggingBehaviour<,>));
            services.AddTransient<ISagaLogger>(ctx => new SerilogSagaLogger(Log.Logger));

            #endregion

            services.AddMassTransit(mtConfig =>
            {
                mtConfig.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(busConfig =>
                {
                    busConfig.Host(_config.RabbitMq.Host, _config.RabbitMq.Port, "/", hostConfig =>
                    {
                        hostConfig.Username(_config.RabbitMq.Username);
                        hostConfig.Password(_config.RabbitMq.Password);
                    });
                    var serviceProvider = provider.CreateScope().ServiceProvider;
                }));
            });
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var bus = app.ApplicationServices.GetService<IBusControl>();
            var busHandle = TaskUtil.Await(() => bus.StartAsync());

            lifetime.ApplicationStopping.Register(() =>
            {
                busHandle.Stop();
            });

            if (_env.IsProduction())
            {
                var context = app.ApplicationServices.GetService<AppDbContext>();
                context.Database.Migrate();
            }
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Legasy Sql API");
            });

            app.UseMiddleware<RequestHandlerMiddleware>();
            app.UseExceptionHandler("/error");

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var mediator = app.ApplicationServices.GetService<IMediator>();
            mediator.Send(new ClearExecutingJobsCommand()).Wait();
        }

        private LegacySqlConfig CreateConfig(IConfiguration config)
        {
            var legacySqlConfig = new LegacySqlConfig();
            config.Bind(legacySqlConfig);
            new LegacySqlConfigValidator().ValidateAndThrow(legacySqlConfig);
            return legacySqlConfig;
        }
    }
}
