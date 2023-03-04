using MediatR;

namespace LegacySql.Commands.PriceAlgorythm.RecalculatePrices
{
    public class RecalculatePricesCommand : IRequest
    {
        public RecalculatePricesCommand(int id)
        {
            Id = id;
        }
        public int Id { get; }
    }
}
