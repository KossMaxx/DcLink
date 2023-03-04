using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Manufacturer;
using MassTransit.Initializers;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ManufacturerRepository : ILegacyManufacturerRepository
    {
        private readonly LegacyDbConnection _db;

        public ManufacturerRepository(LegacyDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Manufacturer>> GetAllAsync()
        {
            var procedure = "dbo.E21_model_get_product_brands";
            var manufacturers = (await _db.Connection.QueryAsync(
                procedure,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600)).Select(m =>
                new Manufacturer(m.Product_ManufactureId, m.Product_Manufacture, m.Product_ManufactureSite));
            return manufacturers;
        }

        public async Task<string> GetAsync(int? legacyId)
        {
            if (!legacyId.HasValue) return null;
            var procedure = "dbo.E21_model_get_product_brand";
            var manufacturer = await _db.Connection.QueryFirstOrDefaultAsync<string>(
                procedure, new
                {
                    Product_ManufactureId = legacyId
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);
            return manufacturer;
        }

        public async Task<int?> GetAsync(string legacyTitle)
        {
            if (string.IsNullOrEmpty(legacyTitle)) return null;
            var procedure = "dbo.E21_model_get_product_brand_by_name";
            var manufacturer = await _db.Connection.QueryFirstOrDefaultAsync<int>(
                procedure, new
                {
                    Product_Manufacture = legacyTitle
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);
            return manufacturer;
        }
    }
}
