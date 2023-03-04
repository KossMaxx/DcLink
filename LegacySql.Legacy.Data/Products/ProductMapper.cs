using LegacySql.Domain.Pictures;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacySql.Legacy.Data.Products
{
    internal class ProductMapper
    {
        public IDictionary<int, Guid?> _productMap;
        public IDictionary<int, Guid?> _productTypeMap;
        public IDictionary<int, Guid?> _manufacturerMap;
        public IDictionary<string, Guid?> _classMap;
        public IDictionary<string, SubtypeData> _subtypeMap;
        public IDictionary<int, string> _countryIsoMap;
        public IDictionary<int, IEnumerable<ProductCategoryParameter>> _parametersMap;
        public IDictionary<int, Guid?> _managerMap;
        Dictionary<int, Dictionary<int, string>> _descriptions;

        public ProductMapper(IDictionary<int, Guid?> productMap,
            IDictionary<int, Guid?> productTypeMap,
            IDictionary<int, Guid?> manufacturerMap,
            IDictionary<string, Guid?> classMap,
            IDictionary<string, SubtypeData> subtypeMap,
            IDictionary<int, string> countryIsoMap,
            IDictionary<int, IEnumerable<ProductCategoryParameter>> parametersMap,
            IDictionary<int, Guid?> managerMap,
            Dictionary<int, Dictionary<int, string>> descriptions)
        {
            _productMap = productMap;
            _productTypeMap = productTypeMap;
            _manufacturerMap = manufacturerMap;
            _classMap = classMap;
            _subtypeMap = subtypeMap;
            _countryIsoMap = countryIsoMap;
            _parametersMap = parametersMap;
            _managerMap = managerMap;
            _descriptions = descriptions;
        }

        public Product Map(ProductData master, IEnumerable<ProductData> items)
        {
            var descriptions = _descriptions.ContainsKey(master.Product_Code) ?
                    _descriptions[master.Product_Code] :
                    null;
            var hasMap = _productMap.ContainsKey(master.Product_Code);
            var productCategory = master.Product_ProductCategory switch
            {
                "Уценка" => "уценка",
                "О" => "о",
                "Спец" => "СПЕЦ",
                _ => master.Product_ProductCategory
            };

            var product = new Product(hasMap)
            {
                Code = new IdMap(master.Product_Code, hasMap ? _productMap[master.Product_Code] : null),
                Brand = master.Product_Brand,
                WorkName = master.Product_WorkName,
                ProductTypeId = master.Product_ProductTypeId.HasValue
                    ? new IdMap(master.Product_ProductTypeId.Value, _productTypeMap.ContainsKey(master.Product_ProductTypeId.Value)
                        ? _productTypeMap[master.Product_ProductTypeId.Value]
                        : null)
                    : null,
                SubtypeId = !string.IsNullOrEmpty(master.Product_Subtype) && _subtypeMap.ContainsKey(master.Product_Subtype)
                    ? new IdMap(_subtypeMap[master.Product_Subtype].LegacyId, _subtypeMap[master.Product_Subtype].ExternalId)
                    : null,
                ProductCategoryId = !string.IsNullOrEmpty(master.Product_ProductCategory)
                    ? new StringIdMap(master.Product_ProductCategory, _classMap.ContainsKey(productCategory)
                        ? _classMap[productCategory]
                        : null)
                    : null,
                ManufactureId = (master.Product_ManufactureId > 0)
                    ? new IdMap(master.Product_ManufactureId, _manufacturerMap.ContainsKey(master.Product_ManufactureId)
                        ? _manufacturerMap[master.Product_ManufactureId]
                        : null)
                    : null,
                VendorCode = master.Product_VendorCode,
                NomenclatureBarcode = master.Product_NomenclatureBarcode,
                NameForPrinting = master.Product_NameForPrinting,
                ProductCountry = _countryIsoMap[master.Product_ProductCountryId],
                BrandCountry = _countryIsoMap[master.Product_BrandCountryId],
                Weight = master.Product_Weight,
                Volume = master.Product_Volume,
                PackageQuantity = master.Product_PackageQuantity,
                Guarantee = master.Product_Guarantee,
                GuaranteeIn = master.Product_GuaranteeIn,
                Unit = master.Product_Unit,
                Vat = master.Product_Vat,
                IsImported = master.Product_IsImported,
                NomenclatureCode = master.Product_NomenclatureCode,
                IsProductIssued = master.Product_IsProductIssued,
                ContentUser = master.Product_ContentUser,
                InStock = master.Product_InStock,
                InReserve = master.Product_InReserve,
                Pending = master.Product_Pending,
                VideoUrl = master.Product_VideoUrl,
                IsDistribution = master.Product_IsDistribution,
                ScanMonitoring = master.Product_ScanMonitoring,
                ScanHotline = master.Product_ScanHotline,
                Game = master.Product_Game,
                ManualRrp = master.Product_ManualRrp,
                NotInvolvedInPricing = master.Product_NotInvolvedInPricing,
                Monitoring = master.Product_Monitoring,
                Price = master.Product_Price,
                Markdown = master.Product_Markdown,
                ManagerId = master.Product_ManagerId > 0
                    ? new IdMap(master.Product_ManagerId, _managerMap.ContainsKey(master.Product_ManagerId) ? _managerMap[master.Product_ManagerId] : null)
                    : null,
                ChangedAt = master.Product_LastChangeDate,
                ManufactureSiteLink = master.Product_ManufactureSiteLink,
                DescriptionRu = descriptions != null && descriptions.ContainsKey(1) ? descriptions[1] : null,
                DescriptionUa = descriptions != null && descriptions.ContainsKey(2) ? descriptions[2] : null,
                CurrencyId = master.Product_Currency,
                Height = master.Product_Height,
                Width = master.Product_Width,
                Depth = master.Product_Depth,
                NonCashProductId = master.Product_NonCashProductId,
                Parameters = _parametersMap.ContainsKey(master.Product_Code) ? _parametersMap[master.Product_Code] : new List<ProductCategoryParameter>(),
                IsProduction = master.Product_Production
            };

            var pictures = new Dictionary<int, PictureData>();
            var video = new Dictionary<int, VideoData>();
            foreach (var productDataItem in items)
            {
                if (productDataItem.Picture != null)
                {
                    pictures.TryAdd(productDataItem.Picture.Product_pic_Id, productDataItem.Picture);
                }

                if (productDataItem.Video != null)
                {
                    video.TryAdd(productDataItem.Video.Product_video_Id, productDataItem.Video);
                }
            }

            product.Pictures = pictures.Values.Select(e => new ProductPicture
                (
                    e.Product_pic_Id,
                    e.Product_pic_Url,
                    e.Product_pic_Date
                ));
            product.Video = video.Values.Select(e => new ProductVideo
                (
                    e.Product_video_Id,
                    e.Product_video_Url,
                    e.Product_video_Date
                ));

            return product;
        }
    }
}
