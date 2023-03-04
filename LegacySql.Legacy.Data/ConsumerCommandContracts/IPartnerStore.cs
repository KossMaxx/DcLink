using System.Data;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Repositories.ConsumerCommandContracts
{
    public interface IPartnerStore
    {
        Task Update(
            int id,
            string title,
            bool isSupplier,
            bool isCustomer,
            bool isCompetitor,
            string email,
            int balanceCurrencyId,
            int? mainManagerId,
            int? responsibleManagerId,
            short? creditDays,
            byte priceValidDays,
            int? marketSegmentId,
            decimal? credit,
            short? surchargePercents,
            short? bonusPercents,
            bool delayOk,
            string deliveryTel,
            string website,
            string contactPerson,
            string contactPersonPhone,
            string address,
            string mobilePhone,
            byte defaultPriceColumn,
            int? departmentId,
            string city,
            int? marketSegmrntationTurnoverId,
            IDbTransaction transaction);
    }
}
