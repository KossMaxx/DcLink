using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Products;
using MediatR;

namespace LegacySql.Commands.Products.ChangeWrongMapping
{
    public class ChangeWrongMappingCommandHandler : IRequestHandler<ChangeWrongMappingCommand>
    {
        private readonly IProductMapRepository _productMapRepository;

        public ChangeWrongMappingCommandHandler(IProductMapRepository productMapRepository)
        {
            _productMapRepository = productMapRepository;
        }

        public async Task<Unit> Handle(ChangeWrongMappingCommand command, CancellationToken cancellationToken)
        {
            var wrongMapping = await _productMapRepository.GetByErpAsync(command.ExternalId);
            var correctMapping = await _productMapRepository.GetByLegacyAsync(command.InnerId);

            if (wrongMapping != null)
            {
                await _productMapRepository.DeleteByIdAsync(wrongMapping.Id);
            }

            if (correctMapping != null)
            {
                correctMapping.MapToExternalId(command.ExternalId);
                await _productMapRepository.SaveAsync(correctMapping, correctMapping.Id);
            }

            return new Unit();
        }
    }
}
