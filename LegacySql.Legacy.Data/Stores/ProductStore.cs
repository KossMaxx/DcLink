using Dapper;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Stores
{
    public class ProductStore : IProductStore
    {
        private readonly IDbConnection _db;

        public ProductStore(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Create(string brand,
            string workName,
            int productTypeId,
            string subtype,
            string productCategory,
            string manufacture,
            int? manufactureId,
            string vendorCode,
            string nomenclatureBarcode,
            string nameForPrinting,
            int productCountryId,
            int brandCountryId,
            double? weight,
            double? volume,
            byte? guarantee,
            byte? guaranteeIn,
            string unit,
            decimal vat,
            bool? isImported,
            string nomenclatureCode,
            bool? isProductIssued,
            bool? isDistribution,
            bool? scanMonitoring,
            bool? scanHotline,
            bool? game,
            bool manualRrp,
            bool notInvolvedInPricing,
            byte monitoring,
            bool price,
            bool? markdown,
            string inet,
            byte? currencyId,
            string managerNickName,
            int? managerId,
            double? height,
            double? width,
            double? depth,
            IDbTransaction transaction)
        {
            var procedure = "dbo.E21_model_modify_product";
            var procedureParams = new
            {
                Product_Brand = brand,
                Product_WorkName = workName,
                Product_ProductTypeId = productTypeId,
                Product_Subtype = subtype,
                Product_ProductCategory = productCategory,
                Product_Manufacture = manufacture,
                Product_ManufactureId = manufactureId,
                Product_VendorCode = vendorCode,
                Product_NomenclatureBarcode = nomenclatureBarcode,
                Product_NameForPrinting = nameForPrinting,
                Product_ProductCountryId = productCountryId,
                Product_BrandCountryId = brandCountryId,
                Product_Weight = weight,
                Product_Volume = volume,
                Product_Guarantee = guarantee,
                Product_GuaranteeIn = guaranteeIn,
                Product_Unit = unit,
                Product_Vat = vat,
                Product_IsImported = isImported,
                Product_NomenclatureCode = nomenclatureCode,
                Product_IsProductIssued = isProductIssued,
                Product_IsDistribution = isDistribution,
                Product_ScanMonitoring = scanMonitoring,
                Product_ScanHotline = scanHotline,
                Product_Game = game,
                Product_ManualRrp = manualRrp,
                Product_NotInvolvedInPricing = notInvolvedInPricing,
                Product_Monitoring = monitoring,
                Product_Price = price,
                Product_Markdown = markdown,
                Product_ManufactureSiteLink = inet,
                Product_Currency = currencyId,
                Product_ManagerNickName = managerNickName,
                Product_ManagerId = managerId,
                Product_Height = height,
                Product_Width = width,
                Product_Depth = depth
            };

            return await _db.QueryFirstOrDefaultAsync<int>(procedure, procedureParams, transaction, 300, CommandType.StoredProcedure);
        }
        public async Task Update(
            int productCode, 
            string brand, 
            string workName, 
            int productTypeId, 
            string subtype, 
            string productCategory, 
            string manufacture, 
            int? manufactureId, 
            string vendorCode, 
            string nomenclatureBarcode, 
            string nameForPrinting, 
            int productCountryId, 
            int brandCountryId, 
            double? weight,
            double? volume, 
            byte? guarantee, 
            byte? guaranteeIn, 
            string unit, 
            decimal vat,
            bool? isImported, 
            string nomenclatureCode, 
            bool? isProductIssued, 
            bool? isDistribution, 
            bool? scanMonitoring, 
            bool? scanHotline, 
            bool? game, 
            bool manualRrp, 
            bool notInvolvedInPricing, 
            byte monitoring, 
            bool price, 
            bool? markdown,
            string inet,
            byte? currencyId, 
            string managerNickName, 
            int? managerId, 
            double? height, 
            double? width, 
            double? depth,
            IDbTransaction transaction)
        {
            var procedure = "dbo.E21_model_modify_product";
            var procedureParams = new
            {
                Product_Code = productCode,
                Product_Brand = brand,
                Product_WorkName = workName,
                Product_ProductTypeId = productTypeId,
                Product_Subtype = subtype,
                Product_ProductCategory = productCategory,
                Product_Manufacture = manufacture,
                Product_ManufactureId = manufactureId,
                Product_VendorCode = vendorCode,
                Product_NomenclatureBarcode = nomenclatureBarcode,
                Product_NameForPrinting = nameForPrinting,
                Product_ProductCountryId = productCountryId,
                Product_BrandCountryId = brandCountryId,
                Product_Weight = weight,
                Product_Volume = volume,
                Product_Guarantee = guarantee,
                Product_GuaranteeIn = guaranteeIn,
                Product_Unit = unit,
                Product_Vat = vat,
                Product_IsImported = isImported,
                Product_NomenclatureCode = nomenclatureCode,
                Product_IsProductIssued = isProductIssued,
                Product_IsDistribution = isDistribution,
                Product_ScanMonitoring = scanMonitoring,
                Product_ScanHotline = scanHotline,
                Product_Game = game,
                Product_ManualRrp = manualRrp,
                Product_NotInvolvedInPricing = notInvolvedInPricing,
                Product_Monitoring = monitoring,
                Product_Price = price,
                Product_Markdown = markdown,
                Product_ManufactureSiteLink = inet,
                Product_Currency = currencyId,
                Product_ManagerNickName = managerNickName,
                Product_ManagerId = managerId,
                Product_Height = height,
                Product_Width = width,
                Product_Depth = depth
            };

            await _db.ExecuteAsync(procedure, procedureParams, transaction, 300, CommandType.StoredProcedure);
        }

        public async Task InsertParameter(int productId, int categoryId, int categoryParamId, IDbTransaction transaction)
        {
            var procedureParams = new
            {
                Product_params_ProductId = productId,
                Product_params_CategoryId = categoryId,
                Product_params_Description = categoryParamId
            };

            var procedure = "dbo.E21_model_modify_product_params_by_id";
            await _db.ExecuteAsync(procedure, procedureParams, transaction, 300, CommandType.StoredProcedure);
        }

        public async Task UpdateParameter(int id, int productId, int categoryId, int categoryParamId, IDbTransaction transaction)
        {
            var procedureParams = new
            {
                Product_params_Id = id,
                Product_params_ProductId = productId,
                Product_params_CategoryId = categoryId,
                Product_params_Description = categoryParamId
            };

            var procedure = "dbo.E21_model_modify_product_params_by_id";
            await _db.ExecuteAsync(procedure, procedureParams, transaction, 300, CommandType.StoredProcedure);
        }

        public async Task UpdateDescriptions(int productId, int languageId, string description, IDbTransaction transaction)
        {
            var procedure = "dbo.E21_model_modify_product_descriptions";
            await _db.ExecuteAsync(
                procedure,
                new
                {
                    Product_desc_ProductId = productId,
                    Product_desc_Language = languageId,
                    Product_desc_Description = description
                },
                transaction, 300, CommandType.StoredProcedure
            );
        }
    }
}
