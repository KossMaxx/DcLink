
using MediatR;

namespace LegacySql.Commands.PriceAlgorythm.SaveErpPriceAlgorythm
{
    public class SaveErpPriceAlgorythmCommand : IRequest<int>
    {
        public SaveErpPriceAlgorythmCommand(ErpPriceAlgorythmDto value, int? id = null)
        {
            Value = value;
            Id = id;
        }
        public ErpPriceAlgorythmDto Value { get; }
        public int? Id { get; }
    }
}
