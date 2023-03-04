using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Manufacturer;
using MediatR;
using MessageBus.Manufacturer.Import;

namespace LegacySql.Consumers.Commands.Manufacturers.SaveErpManufacturer
{
    public class SaveErpManufacturerCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpManufacturerDto>>
    {
        private readonly ErpManufacturerSaver _erpManufacturerSaver;
        private readonly IManufacturerMapRepository _manufacturerMapRepository;

        public SaveErpManufacturerCommandHandler(ErpManufacturerSaver erpManufacturerSaver, IManufacturerMapRepository manufacturerMapRepository)
        {
            _erpManufacturerSaver = erpManufacturerSaver;
            _manufacturerMapRepository = manufacturerMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpManufacturerDto> command, CancellationToken cancellationToken)
        {
            var manufacturer = command.Value;
            var map = await _manufacturerMapRepository.GetByErpAsync(manufacturer.Id);
            _erpManufacturerSaver.InitErpObject(manufacturer, map);
            await _erpManufacturerSaver.Save(command.MessageId);
            return new Unit();
        }

        //private async Task Update(ErpManufacturerDto manufacturer, ManufacturerMap map)
        //{
        //    _db.Open();
        //    using var transaction = _db.BeginTransaction();
        //    try
        //    {
                
        //        var updateProductQuery = @"update [dbo].[Товары] 
        //                                 set [manufacture]=@NewTitle
        //                                 where trim([manufacture])=@OldTitle";
        //        var updateKolonkaByKlientQuery = @"update [dbo].[kolonkaByKlient] 
        //                                         set [vendor]=@NewTitle
        //                                         where trim([vendor])=@OldTitle";
        //        var updatePriceAlgoritmDetailsQuery = @"update [dbo].[PriceAlgoritmDetails] 
        //                                              set [Vendor]=@NewTitle
        //                                              where trim([Vendor])=@OldTitle";
        //        var queryObject = new
        //        {
        //            NewTitle = manufacturer.Title,
        //            OldTitle = map.LegacyTitle
        //        };
        //        await _db.ExecuteAsync(updateProductQuery, queryObject, transaction);
        //        await _db.ExecuteAsync(updateKolonkaByKlientQuery, queryObject, transaction);
        //        await _db.ExecuteAsync(updatePriceAlgoritmDetailsQuery, queryObject, transaction);

        //        transaction.Commit();

        //        await _manufacturerMapRepository.SaveAsync(new ManufacturerMap(map.MapId,manufacturer.Title,map.ExternalMapId,map.Id), map.Id);
        //    }
        //    catch (Exception e)
        //    {
        //        transaction.Rollback();
        //        throw e;
        //    }
        //    finally
        //    {
        //        _db.Close();
        //    }
        //}
    }
}
