using Dapper;
using LegacySql.Legacy.Data.Repositories.ConsumerCommandContracts;
using System.Data;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Stores
{
    public class PartnerStore : IPartnerStore
    {
        private readonly IDbConnection _db;

        public PartnerStore(IDbConnection db)
        {
            _db = db;
        }

        public async Task Update(int id, string title, bool isSupplier, bool isCustomer, bool isCompetitor, string email, int balanceCurrencyId, int? mainManagerId, int? responsibleManagerId, short? creditDays, byte priceValidDays, int? marketSegmentId, decimal? credit, short? surchargePercents, short? bonusPercents, bool delayOk, string deliveryTel, string website, string contactPerson, string contactPersonPhone, string address, string mobilePhone, byte defaultPriceColumn, int? departmentId, string city, int? marketSegmrntationTurnoverId, IDbTransaction transaction)
        {
            var procedure = "dbo.E21_pkg_modify_client";
            var procedureParams = new
            {
                SupplierCode = id,
                Title = title,
                OnlySuperReports = false,
                IsSupplier = isSupplier,
                IsCustomer = isCustomer,
                IsCompetitor = isCompetitor,
                Email = email,
                MasterID = id,
                BalanceCurrencyId = balanceCurrencyId,
                Department = departmentId,
                CreditDays = creditDays,
                PriceValidDays = priceValidDays,
                MainManagerId = mainManagerId,
                ResponsibleManagerId = responsibleManagerId,
                MarketSegmentId = marketSegmentId,
                Credit = credit,
                SurchargePercents = surchargePercents,
                BonusPercents = bonusPercents,
                DelayOk = delayOk,
                DeliveryTel = deliveryTel,
                City = city,
                Website = website,
                ContactPerson = contactPerson,
                ContactPersonPhone = contactPersonPhone,
                Address = address,
                MobilePhone = mobilePhone,
                DefaultPriceColumn = defaultPriceColumn,
                MarketSegmentationTurnover = marketSegmrntationTurnoverId
            };

            await _db.ExecuteAsync(procedure, procedureParams, transaction, null, CommandType.StoredProcedure);
        }
    }
}
