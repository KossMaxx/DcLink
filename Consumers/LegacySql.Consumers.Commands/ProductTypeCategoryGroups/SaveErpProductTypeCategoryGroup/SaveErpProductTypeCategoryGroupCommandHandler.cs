using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ProductTypeCategoryGroups.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ProductTypeCategoryGroups.SaveErpProductTypeCategoryGroup
{
    public class SaveErpProductTypeCategoryGroupCommandHandler : IRequestHandler<BaseSaveErpCommand<ProductTypeCategoryGroupErpDto>>
    {
        private readonly IProductTypeCategoryGroupMapRepository _productTypeCategoryGroupMapRepository;
        private readonly ErpProductTypeCategoryGroupSaver _erpProductTypeCategoryGroupSaver;

        public SaveErpProductTypeCategoryGroupCommandHandler(
            IProductTypeCategoryGroupMapRepository productTypeCategoryGroupMapRepository,
            ErpProductTypeCategoryGroupSaver erpProductTypeCategoryGroupSaver)
        {
            _productTypeCategoryGroupMapRepository = productTypeCategoryGroupMapRepository;
            _erpProductTypeCategoryGroupSaver = erpProductTypeCategoryGroupSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ProductTypeCategoryGroupErpDto> command, CancellationToken cancellationToken)
        {
            var type = command.Value;
            var productTypeCategoryGroupMap = await _productTypeCategoryGroupMapRepository.GetByErpAsync(type.Id);
            _erpProductTypeCategoryGroupSaver.InitErpObject(type, productTypeCategoryGroupMap);
            if (productTypeCategoryGroupMap != null)
            {
                await _erpProductTypeCategoryGroupSaver.Update(command.MessageId);
            }
            else
            {
                await _erpProductTypeCategoryGroupSaver.Create(command.MessageId);
            }
            return new Unit();
        }
    }
}
