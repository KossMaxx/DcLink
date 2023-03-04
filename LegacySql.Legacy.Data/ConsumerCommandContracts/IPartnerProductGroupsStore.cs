using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.ConsumerCommandContracts
{
    public interface IPartnerProductGroupsStore
    {
        Task<int> Create(string title);
        Task Update(int id, string title);
    }
}
