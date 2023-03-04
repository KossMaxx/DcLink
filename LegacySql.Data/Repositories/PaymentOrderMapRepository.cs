using LegacySql.Data.Models;
using LegacySql.Domain.BankPayments;

namespace LegacySql.Data.Repositories
{
    public class PaymentOrderMapRepository : BaseMapRepository<PaymentOrderMapEF>, IPaymentOrderMapRepository
    {
        public PaymentOrderMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
