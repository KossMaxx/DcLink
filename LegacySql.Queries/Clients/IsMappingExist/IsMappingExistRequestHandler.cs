using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Clients;
using MediatR;

namespace LegacySql.Queries.Clients.IsMappingExist
{
    public class IsMappingExistRequestHandler : IRequestHandler<IsMappingExistRequest, bool>
    {
        private readonly IClientMapRepository _clientMapRepository;

        public IsMappingExistRequestHandler(IClientMapRepository clientMapRepository)
        {
            _clientMapRepository = clientMapRepository;
        }

        public async Task<bool> Handle(IsMappingExistRequest request, CancellationToken cancellationToken)
        {
             return await _clientMapRepository.IsMappingExist(request.Id);
        }
    }
}
