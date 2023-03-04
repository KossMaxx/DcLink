using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Employees;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;

namespace LegacySql.Commands.PriceAlgorythm.SaveErpPriceAlgorythm
{
    public class ErpPriceAlgorythmSaver
    {
        private readonly IDbConnection _db;
        private ErpPriceAlgorythmDto _priceAlgorythm;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IManufacturerMapRepository _vendorMapRepository;
        private ExternalMap _managerMapping;
        private ExternalMap _clientMapping;
        private Dictionary<Guid, long> _detailProductTypeIds;
        private Dictionary<Guid, string> _detailVendorTitles;
        private Dictionary<Guid, long> _settingClientIds;
        private IDbTransaction _transaction;

        public ErpPriceAlgorythmSaver(IDbConnection db, 
            IProductTypeMapRepository productTypeMapRepository, 
            IClientMapRepository clientMapRepository, 
            IEmployeeMapRepository employeeMapRepository, 
            IManufacturerMapRepository vendorMapRepository)
        {
            _db = db;
            _productTypeMapRepository = productTypeMapRepository;
            _clientMapRepository = clientMapRepository;
            _employeeMapRepository = employeeMapRepository;
            _vendorMapRepository = vendorMapRepository;
        }

        public void InitErpObject(ErpPriceAlgorythmDto priceAlgorythm)
        {
            _priceAlgorythm = priceAlgorythm;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            if (_priceAlgorythm.ClientGuid.HasValue)
            {
                _clientMapping = await _clientMapRepository.GetByErpAsync(_priceAlgorythm.ClientGuid.Value);
                if (_clientMapping == null)
                {
                    why.Append($"Не найден маппинг для ClientGuid:{_priceAlgorythm.ClientGuid}\n");
                }
            }

            _managerMapping = await _employeeMapRepository.GetByErpAsync(_priceAlgorythm.ManagerGuid);
            if (_managerMapping == null)
            {
                why.Append($"Не найден маппинг для ManagerGuid:{_priceAlgorythm.ManagerGuid}\n");
            }

            if (_priceAlgorythm.Details != null)
            {
                //Details
                _detailProductTypeIds = new Dictionary<Guid, long>();
                foreach (var detail in _priceAlgorythm.Details)
                {
                    if (!_detailProductTypeIds.ContainsKey(detail.ProductTypeGuid))
                    {
                        var map = await _productTypeMapRepository.GetByErpAsync(detail.ProductTypeGuid);
                        if (map == null)
                        {
                            why.Append($"Не найден маппинг для detail.ProductTypeGuid:{detail.ProductTypeGuid}\n");
                        }
                        else
                        {
                            _detailProductTypeIds[detail.ProductTypeGuid] = map.LegacyId;
                        }
                    }
                }

                _detailVendorTitles = new Dictionary<Guid, string>();
                foreach (var detail in _priceAlgorythm.Details)
                {
                    if (detail.VendorGuid.HasValue && !_detailVendorTitles.ContainsKey(detail.VendorGuid.Value))
                    {
                        var map = await _vendorMapRepository.GetByErpAsync(detail.VendorGuid.Value);
                        if (map == null)
                        {
                            why.Append($"Не найден маппинг для detail.VendorGuid:{detail.VendorGuid}\n");
                        }
                        else
                        {
                            _detailVendorTitles[detail.VendorGuid.Value] = map.LegacyTitle;
                        }
                    }
                }
            }

            if (_priceAlgorythm.Settings != null)
            {
                //Setting
                _settingClientIds = new Dictionary<Guid, long>();
                foreach (var setting in _priceAlgorythm.Settings)
                {
                    if (setting.ClientGuid.HasValue && !_settingClientIds.ContainsKey(setting.ClientGuid.Value))
                    {
                        var map = await _clientMapRepository.GetByErpAsync(setting.ClientGuid.Value);
                        if (map == null)
                        {
                            why.Append($"Не найден маппинг для setting.ClientGuid:{setting.ClientGuid}\n");
                        }
                        else
                        {
                            _settingClientIds[setting.ClientGuid.Value] = map.LegacyId;
                        }
                    }
                }
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task<int> SaveErpObject(int? id)
        {
            var managerLogin = await GetManagerLogin();

            _db.Open();
            using var transaction = _db.BeginTransaction();
            _transaction = transaction;
            try
            {
                int priceAlgorythmId;
                if (id.HasValue)
                {
                    await UpdatePriceAlgorythm(id.Value, managerLogin);
                    priceAlgorythmId = id.Value;
                    await DeleteRemovedPriceAlgorythmDetails(priceAlgorythmId);
                    await DeleteRemovedPriceAlgorythmSettings(priceAlgorythmId);
                }
                else
                {
                    priceAlgorythmId = await CreatePriceAlgorythm(managerLogin);
                }

                await UpdatePriceAlgorythmDetails(priceAlgorythmId);
                await UpdatePriceAlgorythmSettings(priceAlgorythmId);

                transaction.Commit();
                return priceAlgorythmId;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                _db.Close();
            }
        }

        private async Task<int> CreatePriceAlgorythm(string managerLogin)
        {
            var insertQuery =
                @"insert into [dbo].[PriceAlgoritm] 
                    ([Nazvanie],
                     [Manager],
                     [Active],
                     [zakazonly],
                     [ActiveRRP],
                     [klientID],
                     [ActivePriceUAH],
                     [zakazonlyRRP],
                     [minRent],
                     [rrpX],
                     [rrpA],
                     [rrpC],
                     [algoritm_nal_type],
                     [minRent_use_sklad],
                     [DoNonDeleteRRP],
                     [opt4x], [opt4a], [opt4type], [opt4competitor], [opt4end], [opt4endX],
                     [opt5x], [opt5a], [opt5type], [opt5competitor], [opt5end], [opt5endX],
                     [opt0x], [opt0a], [opt0type], [opt0competitor], [opt0end], [opt0endX],
                     [opt1x], [opt1a], [opt1type], [opt1competitor], [opt1end], [opt1endX],
                     [optIx], [optIa], [optItype], [optIcompetitor], [optIend], [optIendX],
                     [priceUAHx], [priceUAHa], [priceUAHtype], [priceUAHcompetitor], [priceUAHend], [priceUAHendX])
                    values (@Title,
                            @ManagerLogin,
                            @Active,
                            @ZakazOnly,
                            @ActiveRrp,
                            @ClientId,
                            @ActivePriceUah,
                            @ZakazOnlyRrp,
                            @MinRent,
                            @RrpX,
                            @RrpA,
                            @RrpC,
                            @AlgorithmNalType,
                            @MinRentUseSklad,
                            @DoNonDeleteRrp,
                            @Opt4X, @Opt4A, @Opt4Type, @Opt4Competitor, @Opt4End, @Opt4EndX,
                            @Opt5X, @Opt5A, @Opt5Type, @Opt5Competitor, @Opt5End, @Opt5EndX,
                            @Opt0X, @Opt0A, @Opt0Type, @Opt0Competitor, @Opt0End, @Opt0EndX,
                            @Opt1X, @Opt1A, @Opt1Type, @Opt1Competitor, @Opt1End, @Opt1EndX,
                            @OptIX, @OptIA, @OptIType, @OptICompetitor, @OptIEnd, @OptIEndX,
                            @PriceUahX, @PriceUahA, @PriceUahType, @PriceUahCompetitor, @PriceUahEnd, @PriceUahEndX);
                    select cast(SCOPE_IDENTITY() as int)";

            var newId = (await _db.QueryAsync<int>(insertQuery, new
            {
                _priceAlgorythm.Title,
                ManagerLogin = managerLogin,
                _priceAlgorythm.Active,
                _priceAlgorythm.ZakazOnly,
                _priceAlgorythm.ActiveRrp,
                ClientId = _clientMapping?.LegacyId,
                _priceAlgorythm.ActivePriceUah,
                _priceAlgorythm.ZakazOnlyRrp,
                _priceAlgorythm.MinRent,
                _priceAlgorythm.RrpX,
                _priceAlgorythm.RrpA,
                _priceAlgorythm.RrpC,
                _priceAlgorythm.AlgorithmNalType,
                _priceAlgorythm.MinRentUseSklad,
                _priceAlgorythm.DoNonDeleteRrp,
                _priceAlgorythm.Opt4X, _priceAlgorythm.Opt4A, _priceAlgorythm.Opt4Type, _priceAlgorythm.Opt4Competitor, _priceAlgorythm.Opt4End, _priceAlgorythm.Opt4EndX,
                _priceAlgorythm.Opt5X, _priceAlgorythm.Opt5A, _priceAlgorythm.Opt5Type, _priceAlgorythm.Opt5Competitor, _priceAlgorythm.Opt5End, _priceAlgorythm.Opt5EndX,
                _priceAlgorythm.Opt0X, _priceAlgorythm.Opt0A, _priceAlgorythm.Opt0Type, _priceAlgorythm.Opt0Competitor, _priceAlgorythm.Opt0End, _priceAlgorythm.Opt0EndX,
                _priceAlgorythm.Opt1X, _priceAlgorythm.Opt1A, _priceAlgorythm.Opt1Type, _priceAlgorythm.Opt1Competitor, _priceAlgorythm.Opt1End, _priceAlgorythm.Opt1EndX,
                _priceAlgorythm.OptIX, _priceAlgorythm.OptIA, _priceAlgorythm.OptIType, _priceAlgorythm.OptICompetitor, _priceAlgorythm.OptIEnd, _priceAlgorythm.OptIEndX,
                _priceAlgorythm.PriceUahX, _priceAlgorythm.PriceUahA, _priceAlgorythm.PriceUahType, _priceAlgorythm.PriceUahCompetitor, _priceAlgorythm.PriceUahEnd, _priceAlgorythm.PriceUahEndX
            }, _transaction)).FirstOrDefault();

            return newId;
        }

        private async Task UpdatePriceAlgorythm(long id, string managerLogin)
        {
            var updateQuery =
                @"update [dbo].[PriceAlgoritm] 
                set   [Nazvanie]=@Title,
                      [Manager]=@ManagerLogin,
                      [Active]=@Active,
                      [zakazonly]=@ZakazOnly,
                      [ActiveRRP]=@ActiveRrp,
                      [klientID]=@ClientId,
                      [ActivePriceUAH]=@ActivePriceUah,
                      [zakazonlyRRP]=@ZakazOnlyRrp,
                      [minRent]=@MinRent,
                      [rrpX]=@RrpX,
                      [rrpA]=@RrpA,
                      [rrpC]=@RrpC,
                      [algoritm_nal_type]=@AlgorithmNalType,
                      [minRent_use_sklad]=@MinRentUseSklad,
                      [DoNonDeleteRRP]=@DoNonDeleteRrp,                                    
                      [opt4x]=@Opt4X, [opt4a]=@Opt4A, [opt4type]=@Opt4Type, [opt4competitor]=@Opt4Competitor, [opt4end]=@Opt4End, [opt4endX]=@Opt4EndX,
                      [opt5x]=@Opt5X, [opt5a]=@Opt5A, [opt5type]=@Opt5Type, [opt5competitor]=@Opt5Competitor, [opt5end]=@Opt5End, [opt5endX]=@Opt5EndX,
                      [opt0x]=@Opt0X, [opt0a]=@Opt0A, [opt0type]=@Opt0Type, [opt0competitor]=@Opt0Competitor, [opt0end]=@Opt0End, [opt0endX]=@Opt0EndX,
                      [opt1x]=@Opt1X, [opt1a]=@Opt1A, [opt1type]=@Opt1Type, [opt1competitor]=@Opt1Competitor, [opt1end]=@Opt1End, [opt1endX]=@Opt1EndX,
                      [optIx]=@OptIX, [optIa]=@OptIA, [optItype]=@OptIType, [optIcompetitor]=@OptICompetitor, [optIend]=@OptIEnd, [optIendX]=@OptIEndX,
                      [priceUAHx]=@PriceUahX, [priceUAHa]=@PriceUahA, [priceUAHtype]=@PriceUahType, [priceUAHcompetitor]=@PriceUahCompetitor, [priceUAHend]=@PriceUahEnd, [priceUAHendX]=@PriceUahEndX
                where [PriceAlgoritmID]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = id,
                _priceAlgorythm.Title,
                ManagerLogin = managerLogin,
                _priceAlgorythm.Active,
                _priceAlgorythm.ZakazOnly,
                _priceAlgorythm.ActiveRrp,
                ClientId = _clientMapping?.LegacyId,
                _priceAlgorythm.ActivePriceUah,
                _priceAlgorythm.ZakazOnlyRrp,
                _priceAlgorythm.MinRent,
                _priceAlgorythm.RrpX,
                _priceAlgorythm.RrpA,
                _priceAlgorythm.RrpC,
                _priceAlgorythm.AlgorithmNalType,
                _priceAlgorythm.MinRentUseSklad,
                _priceAlgorythm.DoNonDeleteRrp,
                _priceAlgorythm.Opt4X, _priceAlgorythm.Opt4A, _priceAlgorythm.Opt4Type, _priceAlgorythm.Opt4Competitor, _priceAlgorythm.Opt4End, _priceAlgorythm.Opt4EndX,
                _priceAlgorythm.Opt5X, _priceAlgorythm.Opt5A, _priceAlgorythm.Opt5Type, _priceAlgorythm.Opt5Competitor, _priceAlgorythm.Opt5End, _priceAlgorythm.Opt5EndX,
                _priceAlgorythm.Opt0X, _priceAlgorythm.Opt0A, _priceAlgorythm.Opt0Type, _priceAlgorythm.Opt0Competitor, _priceAlgorythm.Opt0End, _priceAlgorythm.Opt0EndX,
                _priceAlgorythm.Opt1X, _priceAlgorythm.Opt1A, _priceAlgorythm.Opt1Type, _priceAlgorythm.Opt1Competitor, _priceAlgorythm.Opt1End, _priceAlgorythm.Opt1EndX,
                _priceAlgorythm.OptIX, _priceAlgorythm.OptIA, _priceAlgorythm.OptIType, _priceAlgorythm.OptICompetitor, _priceAlgorythm.OptIEnd, _priceAlgorythm.OptIEndX,
                _priceAlgorythm.PriceUahX, _priceAlgorythm.PriceUahA, _priceAlgorythm.PriceUahType, _priceAlgorythm.PriceUahCompetitor, _priceAlgorythm.PriceUahEnd, _priceAlgorythm.PriceUahEndX
            }, _transaction);
        }

        private async Task DeleteRemovedPriceAlgorythmDetails(long priceAlgorythmId)
        {
            if (_priceAlgorythm.Details == null)
            {
                return;
            }

            var selectQuery = @"select [PriceAlgoritmDetailsID]
                                from [dbo].[PriceAlgoritmDetails]
                                where [PriceAlgoritmID]=@Id";
            var curDetails = (await _db.QueryAsync<int>(selectQuery, new {Id = priceAlgorythmId}, _transaction)).ToList();
            if (!curDetails.Any())
            {
                return;
            }
            var deleteQuery = @"delete from [dbo].[PriceAlgoritmDetails]
                                where [PriceAlgoritmDetailsID] in @Ids";
            await _db.ExecuteAsync(deleteQuery, new {Ids = curDetails.Except(_priceAlgorythm.Details.Select(i => i.Id))}, _transaction);
        }

        private async Task DeleteRemovedPriceAlgorythmSettings(long priceAlgorythmId)
        {
            if (_priceAlgorythm.Settings == null)
            {
                return;
            }
            
            var selectQuery = @"select [PriceAlgoritmSuplListID]
                                from [dbo].[PriceAlgoritmSuplList]
                                where [PriceAlgoritmID]=@Id";
            var curSettings = (await _db.QueryAsync<int>(selectQuery, new {Id = priceAlgorythmId}, _transaction)).ToList();
            if (!curSettings.Any())
            {
                return;
            }
            var deleteQuery = @"delete from [dbo].[PriceAlgoritmSuplList]
                                where [PriceAlgoritmSuplListID] in @Ids";
            await _db.ExecuteAsync(deleteQuery, new {Ids = curSettings.Except(_priceAlgorythm.Settings.Select(i => i.Id))}, _transaction);
        }

        private async Task UpdatePriceAlgorythmDetails(long priceAlgorythmId)
        {
            if (_priceAlgorythm.Details == null)
            {
                return;
            }

            var insertQuery = @"insert into [dbo].[PriceAlgoritmDetails] 
                                    ([PriceAlgoritmID],
                                     [CategoryID],
                                     [Vendor],
                                     [klas])
                                 values (@PriceAlgorythmId,
                                        @ProductTypeId,
                                        @Vendor,
                                        @Class);";

            var updateQuery = @"update [dbo].[PriceAlgoritmDetails] 
                                 set   [CategoryID]=@ProductTypeId,                           
                                       [klas]=@Class,
                                       [Vendor]=@Vendor
                                 where [PriceAlgoritmDetailsID]=@PriceAlgorythmDetailsId";

            foreach (var detail in _priceAlgorythm.Details)
            {
                string query = detail.Id > 0 ? updateQuery : insertQuery;

                await _db.QueryAsync<int>(query, new
                {
                    PriceAlgorythmDetailsId = detail.Id,
                    priceAlgorythmId,
                    ProductTypeId = _detailProductTypeIds[detail.ProductTypeGuid],
                    Vendor = detail.VendorGuid.HasValue ? _detailVendorTitles[detail.VendorGuid.Value] : "все",
                    Class = string.IsNullOrEmpty(detail.Class) ? "Все" : detail.Class,
                }, _transaction);
            }
        }

        private async Task UpdatePriceAlgorythmSettings(long priceAlgorythmId)
        {
            if (_priceAlgorythm.Settings == null)
            {
                return;
            }

            var insertQuery = @"insert into [dbo].[PriceAlgoritmSuplList] 
                                      ([PriceAlgoritmID],
                                       [klientID],
                                       [rrp_s],
                                       [rdp_s],
                                       [xrate],
                                       [price_s],
                                       [competitor_opt],
                                       [competitor_rozn],
                                       [competitor_1],
                                       [competitor_1rozn])
                                   values (@PriceAlgorythmId,
                                          @ClientId,
                                          @Rrp,
                                          @Rdp,
                                          @XRate,
                                          @Price,
                                          @CompetitorOpt,
                                          @CompetitorRozn,
                                          @Competitor1,
                                          @Competitor1Rozn);";

            var updateQueryTemplate = @"update [dbo].[PriceAlgoritmSuplList] 
                                    set   [rrp_s]=@Rrp,
                                          [rdp_s]=@Rdp,
                                          [xrate]=@XRate,
                                          [price_s]=@Price,
                                          [competitor_opt]=@CompetitorOpt,
                                          [competitor_rozn]=@CompetitorRozn,
                                          [competitor_1]=@Competitor1,
                                          [competitor_1rozn]=@Competitor1Rozn
                                          {0}
                                    where [PriceAlgoritmSuplListID]=@PriceAlgorythmSettingsId";
            var clientUpdateExpr = ",[klientID]=@ClientId";

            var updateQueryAll = string.Format(updateQueryTemplate, clientUpdateExpr);
            var updateQueryWithoutClientId = string.Format(updateQueryTemplate, string.Empty);

            foreach (var setting in _priceAlgorythm.Settings)
            {
                string query = setting.Id > 0
                    ? setting.ClientGuid.HasValue ? updateQueryAll : updateQueryWithoutClientId
                    : insertQuery;

                await _db.QueryAsync<int>(query, new
                {
                    PriceAlgorythmSettingsId = setting.Id,
                    priceAlgorythmId,
                    ClientId = setting.ClientGuid.HasValue ? _settingClientIds[setting.ClientGuid.Value] : 0,
                    setting.Rrp,
                    setting.Rdp,
                    setting.XRate,
                    setting.Price,
                    setting.CompetitorOpt,
                    setting.CompetitorRozn,
                    setting.Competitor1,
                    setting.Competitor1Rozn,
                }, _transaction);
            }
        }

        private async Task<string> GetManagerLogin()
        {
            var sql = @"select uuu as ManagerLogin
                        from Сотрудники
                        where КодСотрудника = @Id";
            var managerLogin = await _db.QueryFirstOrDefaultAsync<string>(sql, new {Id = _managerMapping.LegacyId});

            if (string.IsNullOrEmpty(managerLogin))
            {
                throw new ArgumentException("Manager must have login!");
            }

            return managerLogin;
        }
    }
}