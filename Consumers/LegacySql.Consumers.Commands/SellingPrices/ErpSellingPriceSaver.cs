using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.SellingPrices;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data;
using LegacySql.Legacy.Data.Models;
using MessageBus.SellingPrices.Import;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Consumers.Commands.SellingPrices
{
    public class ErpSellingPriceSaver
    {
        private readonly IDbConnection _db;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly LegacyDbContext _legacyDb;
        private ExternalMap _productMapping;
        private ErpSellingPriceDto _price;

        public ErpSellingPriceSaver(IDbConnection db, 
            LegacyDbContext legacyDb, 
            IErpChangedRepository erpChangedRepository)
        {
            _db = db;
            _legacyDb = legacyDb;
            _erpChangedRepository = erpChangedRepository;
        }

        public void InitErpObject(ErpSellingPriceDto price, ExternalMap productMapping)
        {
            _productMapping = productMapping;
            _price = price;
        }

        public MappingInfo GetMappingInfo(ErpSellingPriceDto price)
        {
            var why = new StringBuilder();
            if (_productMapping == null)
            {
                why.Append($"Маппинг товара id:{price.ProductId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Update()
        {
            var priceColumnTitle = new SellingPriceColumn(_price.ColumnId).Title;
            if (_price.PaymentType == PaymentTypes.Cash)
            {
                await UpdatePrice(priceColumnTitle);
            }

            var cashlessProductCardId = await GetCashlessProductCardId();
            if (_price.PaymentType == PaymentTypes.Cashless && cashlessProductCardId.HasValue)
            {
                await UpdatePrice(priceColumnTitle, cashlessProductCardId);
            }

            if (_price.PaymentType == PaymentTypes.Cashless && !cashlessProductCardId.HasValue)
            {
                await UpdateCashlessPrice();
            }

            await SaveInErpChange();
        }

        private async Task<int?> GetCashlessProductCardId()
        {
            var selectCashlessProductCardsIdSqlQuery = @"select [beznal_tovID] from [dbo].[Товары]
                                                       where [КодТовара]=@ProductId";
            return await _db.QueryFirstOrDefaultAsync<int?>(selectCashlessProductCardsIdSqlQuery, new
            {
                ProductId = _productMapping.LegacyId
            });
        }

        private async Task UpdatePrice(string priceColumnTitle, int? productId = null)
        {
            var updateQuery = $@"update [dbo].[Товары] 
                                set [{priceColumnTitle}]=@Price
                                where [КодТовара]=@ProductId";
            await _db.ExecuteAsync(updateQuery, new
            {
                ProductId = productId ?? _productMapping.LegacyId,
                _price.Price,
            });
        }

        private async Task UpdateCashlessPrice()
        {
            var cashProduct = await GetProductById();

            var cashlessProduct = cashProduct.GetCopy();
            cashlessProduct.WorkName = $"{cashProduct.WorkName}_бн";
            cashlessProduct.VendorCode = $"{cashProduct.VendorCode}_бн";
            cashlessProduct.Price = false;
            cashlessProduct.SetPrice(_price.ColumnId, _price.Price);

            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertCashlessProductSqlQuery = @"insert into [dbo].[Товары] 
                                              ([Марка]
                                              ,[Позиция]
                                              ,[КодТипа]
                                              ,[Цена0]
                                              ,[Цена1]
                                              ,[Цена2]
                                              ,[Цена3]
                                              ,[Цена4]
                                              ,[Цена5]
                                              ,[изм]
                                              ,[нал_ф]
                                              ,[SS]
                                              ,[PRICE]
                                              ,[war]
                                              ,[manufacture]
                                              ,[датаизм]
                                              ,[подтип]
                                              ,[ProductManager]
                                              ,[обьем]
                                              ,[вес]
                                              ,[warin]
                                              ,[artikul]
                                              ,[kolpak]
                                              ,[нал_ожид]
                                              ,[нал_резерв]
                                              ,[ЦенаИ]
                                              ,[klas]
                                              ,[contentOK]
                                              ,[contentUser]
                                              ,[KodZED]
                                              ,[videoURL]
                                              ,[EAN]
                                              ,[ВалютаТовара]
                                              ,[уценка]
                                              ,[PriceMinBNuah]
                                              ,[game]
                                              ,[VAT]
                                              ,[RRP]
                                              ,[DataLastPriceChange]
                                              ,[manual_rrp]
                                              ,[beznal_tovID]
                                              ,[о_импрот]
                                              ,[pricealgoritm_ignore]
                                              ,[scan_hotline]
                                              ,[scan_monitoring]
                                              ,[is_distribution]
                                              ,[width]
                                              ,[height]
                                              ,[depth]
                                              ,[countryOfRegistration_ID]
                                              ,[countryOfOrigin_ID]
                                              ,[beznal])
                                            values (
                                                @Brand,
                                                @WorkName,
                                                @ProductTypeId,
                                                @Price0,
                                                @Price1,
                                                @DistributorPrice,
                                                @RRPPrice,
                                                @SpecialPrice,
                                                @MinPrice,
                                                @Unit,
                                                @InStock,
                                                @FirstCost,
                                                @Price,
                                                @Guarantee,
                                                @Manufacture,
                                                @LastChangeDate,
                                                @Subtype,
                                                @ManagerNickName,
                                                @Volume,
                                                @Weight,
                                                @GuaranteeIn,  
                                                @VendorCode, 
                                                @PackageQuantity, 
                                                @Pending,
                                                @InReserve,   
                                                @InternetPrice,
                                                @ProductCategory,
                                                @IsProductIssued,
                                                @ContentUser,
                                                @NomenclatureCode,
                                                @VideoUrl,
                                                @NomenclatureBarcode,
                                                @Currency,
                                                @Markdown,
                                                @PriceMinBnuah,
                                                @Game,                                            
                                                @Vat,
                                                @Monitoring,
                                                @DataLastPriceChange,
                                                @ManualRrp,
                                                @NonCashProductId,
                                                @IsImported,
                                                @NotInvolvedInPricing,
                                                @ScanHotline,
                                                @ScanMonitoring,
                                                @IsDistribution,
                                                @Width,                                                
                                                @Height,                                                
                                                @Depth,
                                                @BrandCountryId,
                                                @ProductCountryId,
                                                @NameForPrinting);
                                             select cast(SCOPE_IDENTITY() as int)";

                var newCashlessProductId = await _db.QueryAsync<int>(insertCashlessProductSqlQuery, cashlessProduct, transaction);

                var updateQuery = @"update [dbo].[Товары] 
                                  set [beznal_tovID]=@CashlessProductId
                                  where [КодТовара]=@ProductId";
                await _db.ExecuteAsync(updateQuery, new
                {
                    ProductId = _productMapping.LegacyId,
                    CashlessProductId = newCashlessProductId
                }, transaction);

                transaction.Commit();
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

        private async Task<ProductEF> GetProductById()
        {
            var selectSqlQuery = @"select [Марка] as Brand
                                          ,[Позиция] as WorkName
                                          ,[КодТипа] as ProductTypeId
                                          ,[Цена0] as Price0
                                          ,[Цена1] as Price1
                                          ,[Цена2] as DistributorPrice
                                          ,[Цена3] as RRPPrice
                                          ,[Цена4] as SpecialPrice
                                          ,[Цена5] as MinPrice
                                          ,[изм] as Unit
                                          ,[нал_ф] as InStock
                                          ,[SS] as FirstCost
                                          ,[PRICE] as Price
                                          ,[war] as Guarantee
                                          ,[manufacture] as Manufacture
                                          ,[датаизм] as LastChangeDate
                                          ,[подтип] as Subtype
                                          ,[ProductManager] as ManagerNickName
                                          ,[обьем] as Volume
                                          ,[вес] as Weight
                                          ,[warin] as GuaranteeIn
                                          ,[artikul] as VendorCode
                                          ,[kolpak] as PackageQuantity
                                          ,[нал_ожид] as Pending
                                          ,[нал_резерв] as InReserve
                                          ,[ЦенаИ] as InternetPrice
                                          ,[klas] as ProductCategory
                                          ,[contentOK] as IsProductIssued
                                          ,[contentUser] as ContentUser
                                          ,[KodZED] as NomenclatureCode
                                          ,[videoURL] as VideoUrl
                                          ,[EAN] as NomenclatureBarcode
                                          ,[ВалютаТовара] as Currency
                                          ,[уценка] as Markdown
                                          ,[PriceMinBNuah] as PriceMinBnuah
                                          ,[game] as Game
                                          ,[VAT] as Vat
                                          ,[RRP] as Monitoring
                                          ,[DataLastPriceChange] as DataLastPriceChange
                                          ,[manual_rrp] as ManualRrp
                                          ,[beznal_tovID] as NonCashProductId
                                          ,[о_импрот] as IsImported
                                          ,[pricealgoritm_ignore] as NotInvolvedInPricing
                                          ,[scan_hotline] as ScanHotline
                                          ,[scan_monitoring] as ScanMonitoring
                                          ,[is_distribution] as IsDistribution
                                          ,[width] as Width
                                          ,[height] as Height
                                          ,[depth] as Depth
                                          ,[countryOfRegistration_ID] as BrandCountryId
                                          ,[countryOfOrigin_ID] as ProductCountryId
                                          ,[beznal] as NameForPrinting
                                    from [dbo].[Товары]
                                    where КодТовара = @ProductId";
            return await _db.QueryFirstOrDefaultAsync<ProductEF>(selectSqlQuery, new
            {
                ProductId = _productMapping.LegacyId
            });
        }

        private async Task SaveInErpChange()
        {
            var selectOrderChangedDateQuery = @"select [DataLastPriceChange] from [dbo].[Товары]
                                                where [КодТовара]=@ProductId";
            var priceChangedDate = await _db.QueryFirstOrDefaultAsync<DateTime?>(selectOrderChangedDateQuery, new
            {
                ProductId = _productMapping.LegacyId

            });

            if (priceChangedDate.HasValue)
            {
                await _erpChangedRepository.Save(
                    _productMapping.LegacyId,
                    priceChangedDate,
                    typeof(SellingPrice).Name
                );
            }
        }
    }
}
