using MediatR;

namespace LegacySql.Commands.SupplierCurrencyRates.PublishSupplierCurrencyRate
{
    public class PublishSupplierCurrencyRateCommand : IRequest
    {
        public PublishSupplierCurrencyRateCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
