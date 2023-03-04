using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Warehouses;
using MediatR;

namespace LegacySql.Commands.Warehouses.UnmapWarehouse
{
    public class UnmapWarehouseCommandHandler : IRequestHandler<UnmapWarehouseCommand>
    {
        private readonly IWarehouseMapRepository _warehouseMapRepository;

        public UnmapWarehouseCommandHandler(IWarehouseMapRepository warehouseMapRepository)
        {
            _warehouseMapRepository = warehouseMapRepository;
        }

        public async Task<Unit> Handle(UnmapWarehouseCommand command, CancellationToken cancellationToken)
        {
            await _warehouseMapRepository.RemoveByErpAsync(command.ErpId);
            return new Unit();
        }
    }
}
