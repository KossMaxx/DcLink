using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Employees;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.PriceConditions;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MessageBus.PriceConditions.Import;

namespace LegacySql.Consumers.Commands.PriceConditions
{
    public class ErpPriceConditionSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IPriceConditionMapRepository _priceConditionMapRepository;
        private readonly IManufacturerMapRepository _manufacturerMapRepository;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private ExternalMap _clientMapping;
        private ExternalMap _productTypeMapping;
        private ExternalMap _priceConditionMapping;
        private ExternalMap _productManagerMapping;
        private ManufacturerMap _manufacturerMap;
        private ErpPriceConditionDto _priceCondition;

        public ErpPriceConditionSaver(IDbConnection db, 
            IClientMapRepository clientMapRepository, 
            IProductTypeMapRepository productTypeMapRepository, 
            IManufacturerMapRepository manufacturerMapRepository, 
            IEmployeeMapRepository employeeMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _productTypeMapRepository = productTypeMapRepository;
            _manufacturerMapRepository = manufacturerMapRepository;
            _employeeMapRepository = employeeMapRepository;
        }

        public void InitErpObject(ErpPriceConditionDto entity, ExternalMap priceConditionMapping, ManufacturerMap manufacturerMap)
        {
            _priceCondition = entity;
            _priceConditionMapping = priceConditionMapping;
            _manufacturerMap = manufacturerMap;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            if (_priceCondition.ClientId.HasValue)
            {
                _clientMapping = await _clientMapRepository.GetByErpAsync(_priceCondition.ClientId.Value);
                if (_clientMapping == null)
                {
                    why.Append($"Маппинг клиента id:{_priceCondition.ClientId.Value} не найден\n");
                }
            }

            if (_priceCondition.ProductTypeId.HasValue)
            {
                _productTypeMapping = await _productTypeMapRepository.GetByErpAsync(_priceCondition.ProductTypeId.Value);
                if (_productTypeMapping == null)
                {
                    why.Append($"Маппинг типа товара id:{_priceCondition.ProductTypeId.Value} не найден\n");
                }
            }

            if (_priceCondition.ProductManagerId.HasValue)
            {
                _productManagerMapping = await _employeeMapRepository.GetByErpAsync(_priceCondition.ProductManagerId.Value);
                if (_productManagerMapping == null)
                {
                    why.Append($"Маппинг менеджера товара id:{_priceCondition.ProductManagerId.Value} не найден\n");
                }
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Create(Guid messageGuid)
        {
            var insertQuery = @"insert into [dbo].[kolonkaByKlient] 
                                    ([date_stamp],
                                    [klientID],
                                    [tip],
                                    [vendor],
                                    [product],
                                    [kolonka],
                                    [validdate],
                                    [prim],
                                    [value],
                                    [percent_value],
                                    [upper_threshold_column],
                                    [product_id])
                                values (@Date,
                                    @ClientId,
                                    @ProductTypeId,
                                    @Vendor,
                                    @ProductManager,
                                    @PriceType,
                                    @DateTo,
                                    @Comment,
                                    @Value,
                                    @PercentValue,
                                    @UpperThresholdPriceType,
                                    @ProductManagerId);
                                    select cast(SCOPE_IDENTITY() as int)";

            var newLegacyId = (await _db.QueryAsync<int>(insertQuery, new
            {
                _priceCondition.Date,
                ClientId = _clientMapping?.LegacyId,
                ProductTypeId = _productTypeMapping?.LegacyId,
                Vendor = _manufacturerMap?.LegacyTitle,
                _priceCondition.ProductManager,
                _priceCondition.PriceType,
                _priceCondition.DateTo,
                _priceCondition.Comment,
                _priceCondition.Value,
                _priceCondition.PercentValue,
                _priceCondition.UpperThresholdPriceType,
                ProductManagerId = _productManagerMapping?.LegacyId
            })).FirstOrDefault();

            await _priceConditionMapRepository.SaveAsync(new ExternalMap(messageGuid, newLegacyId, _priceCondition.Id));
        }

        public async Task Update()
        {
            var updateQuery = @"update [dbo].[kolonkaByKlient] 
                                set   [date_stamp]=@Date,
                                      [klientID]=@ClientId,
                                      [tip]=@ProductTypeId,
                                      [vendor]=@Vendor,
                                      [product]=@ProductManager,
                                      [kolonka]=@PriceType,
                                      [validdate]=@DateTo,
                                      [prim]=@Comment,
                                      [value]=@Value,
                                      [percent_value]=@PercentValue,
                                      [upper_threshold_column]=@UpperThresholdPriceType,
                                      [product_id]=@ProductManagerId
                                where [id]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = _priceConditionMapping.LegacyId,
                _priceCondition.Date,
                ClientId = _clientMapping?.LegacyId,
                ProductTypeId = _productTypeMapping?.LegacyId,
                Vendor = _manufacturerMap?.LegacyTitle,
                _priceCondition.ProductManager,
                _priceCondition.PriceType,
                _priceCondition.DateTo,
                _priceCondition.Comment,
                _priceCondition.Value,
                _priceCondition.PercentValue,
                _priceCondition.UpperThresholdPriceType,
                ProductManagerId = _productManagerMapping?.LegacyId
            });
        }
    }
}
