using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.ConsumerCommandContracts
{
    public interface ISegmentationTurnoverStore
    {
        Task<int> Create(string title);
        Task Update(int id, string title);
    }
}
