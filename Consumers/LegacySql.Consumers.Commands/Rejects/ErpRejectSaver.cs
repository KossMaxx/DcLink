using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Products;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using MassTransit;
using MessageBus.Rejects.Export;
using MessageBus.Rejects.Export.Change;
using MessageBus.Rejects.Import;

namespace LegacySql.Consumers.Commands.Rejects
{
    public class ErpRejectSaver
    {
        private readonly IDbConnection _db;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IRejectMapRepository _rejectMapRepository;
        private readonly IPurchaseMapRepository _purchaseMapRepository;
        private readonly IBus _bus;
        private ExternalMap _rejectMapping;
        private ErpRejectDto _reject;
        private ExternalMap _warehouseMapping;
        private ExternalMap _clientMapping;
        private ExternalMap _clientOrderMapping;
        private ExternalMap _supplierIdMapping;
        private ExternalMap _productMapping;
        private ExternalMap _productTypeMapping;
        private ExternalMap _purchaseMapping;
        private ExternalMap _supplierProductMapping;

        public ErpRejectSaver(IWarehouseMapRepository warehouseMapRepository,
            IProductMapRepository productMapRepository,
            IClientMapRepository clientMapRepository,
            IProductTypeMapRepository productTypeMapRepository,
            IClientOrderMapRepository clientOrderMapRepository,
            IRejectMapRepository rejectMapRepository,
            IDbConnection db,
            IPurchaseMapRepository purchaseMapRepository, 
            IBus bus)
        {
            _warehouseMapRepository = warehouseMapRepository;
            _productMapRepository = productMapRepository;
            _clientMapRepository = clientMapRepository;
            _productTypeMapRepository = productTypeMapRepository;
            _clientOrderMapRepository = clientOrderMapRepository;
            _rejectMapRepository = rejectMapRepository;
            _db = db;
            _purchaseMapRepository = purchaseMapRepository;
            _bus = bus;
        }

        public void InitErpObject(ErpRejectDto reject, ExternalMap rejectMapping)
        {
            _reject = reject;
            _rejectMapping = rejectMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();


            _warehouseMapping = await _warehouseMapRepository.GetByErpAsync(_reject.WarehouseId);
            if (_warehouseMapping == null)
            {
                why.Append($"Не найден маппинг для WarehouseId:{_reject.WarehouseId}\n");
            }

            _productMapping = await _productMapRepository.GetByErpAsync(_reject.ProductId);
            if (_productMapping == null)
            {
                why.Append($"Не найден маппинг для ProductId:{_reject.ProductId}\n");
            }

            if (_reject.SupplierProductId.HasValue)
            {
                _supplierProductMapping = await _productMapRepository.GetByErpAsync(_reject.SupplierProductId.Value);
                if (_supplierProductMapping == null)
                {
                    why.Append($"Не найден маппинг для SupplierProductId:{_reject.SupplierProductId}\n");
                }
            }

            if (_reject.ClientId.HasValue)
            {
                _clientMapping = await _clientMapRepository.GetByErpAsync(_reject.ClientId.Value);
                if (_clientMapping == null)
                {
                    why.Append($"Не найден маппинг для ClientId:{_reject.ClientId}\n");
                }
            }

            if (_reject.ClientOrderId.HasValue)
            {
                _clientOrderMapping = await _clientOrderMapRepository.GetByErpAsync(_reject.ClientOrderId.Value);
                if (_clientOrderMapping == null)
                {
                    why.Append($"Не найден маппинг для ClientOrderId:{_reject.ClientOrderId}\n");
                }
            }

            if (_reject.SupplierId.HasValue)
            {
                _supplierIdMapping = await _clientMapRepository.GetByErpAsync(_reject.SupplierId.Value);
                if (_supplierIdMapping == null)
                {
                    why.Append($"Не найден маппинг для SupplierId:{_reject.SupplierId}\n");
                }
            }

            if (_reject.ProductTypeId.HasValue)
            {
                _productTypeMapping = await _productTypeMapRepository.GetByErpAsync(_reject.ProductTypeId.Value);
                if (_productTypeMapping == null)
                {
                    why.Append($"Не найден маппинг для ProductTypeId:{_reject.ProductTypeId}\n");
                }
            }

            if (_reject.PurchaseId.HasValue)
            {
                _purchaseMapping = await _purchaseMapRepository.GetByErpAsync(_reject.PurchaseId.Value);
                if (_purchaseMapping == null)
                {
                    why.Append($"Не найден маппинг для PurchaseId:{_reject.PurchaseId}\n");
                }
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task SaveErpObject(Guid messageId)
        {
            if (_rejectMapping == null)
            {
                var newRejectId = await Create();
                await _rejectMapRepository.SaveAsync(new ExternalMap(messageId, newRejectId, _reject.Id));
                await PublishNewReject(newRejectId);
            }
            else
            {
                await Update();
            }
        }

        private async Task PublishNewReject(int id)
        {
            var rejectDto = MapToDto(id);
            await _bus.Publish(new ChangeLegacyRejectMessage
            {
                SagaId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                Value = rejectDto,
                ErpId = _reject.Id
            });
        }

        private RejectDto MapToDto(int id)
        {
            return new RejectDto
            {
                Number = id,
                CreatedAt = _reject.CreatedAt,
                Date = _reject.CreatedAt ?? DateTime.Now,
                SerialNumber = _reject.SerialNumber,
                ClientTitle = _reject.ClientTitle,
                ClientId = _reject.ClientId,
                StatusForClient = _reject.StatusForClient,
                WarehouseId = _reject.WarehouseId,
                ResponsibleForStatus = _reject.ResponsibleForStatus,
                RepairType = _reject.RepairType,
                DefectDescription = _reject.DefectDescription,
                KitDescription = _reject.KitDescription,
                ProductStatusDescription = _reject.ProductStatusDescription,
                Notes = _reject.Notes,
                ProductStatus = _reject.ProductStatus,
                ClientOrderId = _reject.ClientOrderId,
                ClientOrderDate = _reject.ClientOrderDate,
                ReceiptDocumentDate = _reject.ReceiptDocumentDate,
                ReceiptDocumentId = _reject.ReceiptDocumentId,
                SupplierId = _reject.SupplierId,
                SupplierTitle = _reject.SupplierTitle,
                PurchasePrice = _reject.PurchasePrice,
                ProductMark = _reject.ProductMark,
                ProductId = _reject.ProductId,
                ProductTypeId = _reject.ProductTypeId,
                PurchaseCurrencyPrice = _reject.PurchaseCurrencyPrice,
                OutgoingWarranty = _reject.OutgoingWarranty,
                DepartureDate = _reject.DepartureDate,
                Amount = 1,
                ClientOrderSqlId = _clientMapping?.LegacyId,
                SupplierDescription = _reject.SupplierDescription,
                ReturnDate = _reject.ReturnDate
            };
        }

        private async Task<int> Create()
        {
            var productMark = await GetProductMark(_productMapping.LegacyId);

            var insertSqlQuery = @"insert into [dbo].[brak] 
                                    ([date_created],
                                    [sernom],
                                    [klient],
                                    [klientID],
                                    [tip_zakr],
                                    [sklad],
                                    [sozdal],
                                    [rma_type],
                                    [akt_defect],
                                    [Комплектность],
                                    [descr],
                                    [прим],
                                    [condition],
                                    [rn_id],
                                    [d2],
                                    [partner_doc_ID],
                                    [dpokupki],
                                    [supl1],
                                    [suplID],
                                    [cost2],
                                    [марка],
                                    [Кодтовара],
                                    [код_типа],
                                    [cost1],
                                    [war_ost_klient],
                                    [d_otpr],
                                    [pn_id],
                                    [descrSupl],
                                    [d_vozvr],
                                    [tip_vozvr],
                                    [юзер],
                                    [marka2],
                                    [sernom2],
                                    [Кодтовара2],
                                    [код_типа2])
                                  values 
                                    (@CreatedAt,
                                    @SerialNumber,
                                    @ClientTitle,
                                    @ClientId,
                                    @StatusForClient,
                                    @WarehouseId,
                                    @ResponsibleForStatus,
                                    @RepairType,
                                    @DefectDescription,
                                    @KitDescription,
                                    @ProductStatusDescription,
                                    @Notes,
                                    @ProductStatus,
                                    @ClientOrderId,
                                    @ClientOrderDate,
                                    @ReceiptDocumentId,
                                    @ReceiptDocumentDate,
                                    @SupplierTitle,
                                    @SupplierId,
                                    @PurchasePrice,
                                    @ProductMark,
                                    @ProductId,
                                    @ProductTypeId,
                                    @PurchaseCurrencyPrice,
                                    @OutgoingWarranty,
                                    @DepartureDate,
                                    @PurchaseId,
                                    @SupplierDescription,
                                    @ReturnDate,
                                    @ReturnType,
                                    @ResponsibleForStatus,
                                    @SupplierProductTitle,
                                    @SupplierSerialNumber,
                                    @SupplierProductId,
                                    @SupplierProductType);
                                 select cast(SCOPE_IDENTITY() as int)";
            return (await _db.QueryAsync<int>(insertSqlQuery, new
            {
                CreatedAt = _reject.CreatedAt,
                SerialNumber = _reject.SerialNumber,
                ClientTitle = _reject.ClientTitle,
                ClientId = _clientMapping?.LegacyId,
                StatusForClient = _reject.StatusForClient,
                WarehouseId = _warehouseMapping.LegacyId,
                ResponsibleForStatus = _reject.ResponsibleForStatus,
                RepairType = _reject.RepairType,
                DefectDescription = _reject.DefectDescription,
                KitDescription = _reject.KitDescription,
                ProductStatusDescription = _reject.ProductStatusDescription,
                Notes = _reject.Notes,
                ProductStatus = _reject.ProductStatus,
                ClientOrderId = _reject.ClientOrderId.HasValue ? _clientOrderMapping?.LegacyId : _reject.ClientOrderSqlId,
                ClientOrderDate = _reject.ClientOrderDate,
                ReceiptDocumentDate = _reject.ReceiptDocumentDate,
                ReceiptDocumentId = _reject.ReceiptDocumentId,
                SupplierId = _supplierIdMapping?.LegacyId,
                SupplierTitle = _reject.SupplierTitle,
                PurchasePrice = _reject.PurchasePrice,
                ProductMark = productMark,
                ProductId = _productMapping.LegacyId,
                ProductTypeId = _productTypeMapping?.LegacyId,
                PurchaseCurrencyPrice = _reject.PurchaseCurrencyPrice,
                OutgoingWarranty = _reject.OutgoingWarranty,
                DepartureDate = _reject.DepartureDate,
                PurchaseId = _reject.PurchaseId.HasValue ? _purchaseMapping?.LegacyId : _reject.PurchaseSqlId,
                SupplierDescription = _reject.SupplierDescription,
                ReturnDate = _reject.ReturnDate,
                ReturnType = _reject.ReturnType,
                SupplierProductTitle = _supplierProductMapping != null 
                    ? await GetProductMark(_supplierProductMapping.LegacyId)
                    : "",
                SupplierSerialNumber = _reject.SupplierSerialNumber,
                SupplierProductId = _supplierProductMapping?.LegacyId,
                SupplierProductType = _supplierProductMapping != null
                    ? await GetProductType(_supplierProductMapping.LegacyId)
                    : null
            })).FirstOrDefault();
        }

        private async Task Update()
        {
            var updateSqlQuery = @"update [dbo].[brak] 
                                   set
                                    [date_created]=@CreatedAt,
                                    [sernom]=@SerialNumber,
                                    [klient]=@ClientTitle,
                                    [klientID]=@ClientId,
                                    [tip_zakr]=@StatusForClient,
                                    [sklad]=@WarehouseId,
                                    [sozdal]=@ResponsibleForStatus,
                                    [rma_type]=@RepairType,
                                    [akt_defect]=@DefectDescription,
                                    [Комплектность]=@KitDescription,
                                    [descr]=@ProductStatusDescription,
                                    [прим]=@Notes,
                                    [condition]=@ProductStatus,
                                    [rn_id]=@ClientOrderId,
                                    [d2]=@ClientOrderDate,
                                    [partner_doc_ID]=@ReceiptDocumentId,
                                    [dpokupki]=@ReceiptDocumentDate,
                                    [supl1]=@SupplierTitle,
                                    [suplID]=@SupplierId,
                                    [cost2]=@PurchasePrice,
                                    [марка]=@ProductMark,
                                    [Кодтовара]=@ProductId,
                                    [код_типа]=@ProductTypeId,
                                    [cost1]=@PurchaseCurrencyPrice,
                                    [war_ost_klient]= @OutgoingWarranty,
                                    [d_otpr]=@DepartureDate,
                                    [pn_id]=@PurchaseId,
                                    [descrSupl]=@SupplierDescription,
                                    [d_vozvr]=@ReturnDate,
                                    [tip_vozvr]=@ReturnType,
                                    [юзер]=@ResponsibleForStatus,
                                    [marka2]=@SupplierProductTitle,
                                    [sernom2]=@SupplierSerialNumber,
                                    [Кодтовара2]=@SupplierProductId,
                                    [replacement_item_id]=@ProductId,
                                    [код_типа2]=@SupplierProductType
                                  where [Brak_ID]=@Id";
            await _db.ExecuteAsync(updateSqlQuery, new
            {
                Id = _rejectMapping.LegacyId,
                CreatedAt = _reject.CreatedAt,
                SerialNumber = _reject.SerialNumber,
                ClientTitle = _reject.ClientTitle,
                ClientId = _clientMapping?.LegacyId,
                StatusForClient = _reject.StatusForClient,
                WarehouseId = _warehouseMapping.LegacyId,
                ResponsibleForStatus = _reject.ResponsibleForStatus,
                RepairType = _reject.RepairType,
                DefectDescription = _reject.DefectDescription,
                KitDescription = _reject.KitDescription,
                ProductStatusDescription = _reject.ProductStatusDescription,
                Notes = _reject.Notes,
                ProductStatus = _reject.ProductStatus,
                ClientOrderId = _reject.ClientOrderId.HasValue ? _clientOrderMapping?.LegacyId : _reject.ClientOrderSqlId,
                ClientOrderDate = _reject.ClientOrderDate,
                ReceiptDocumentDate = _reject.ReceiptDocumentDate,
                ReceiptDocumentId = _reject.ReceiptDocumentId,
                SupplierId = _supplierIdMapping?.LegacyId,
                SupplierTitle = _reject.SupplierTitle,
                PurchasePrice = _reject.PurchasePrice,
                ProductMark = await GetProductMark(_productMapping.LegacyId),
                ProductId = _productMapping.LegacyId,
                ProductTypeId = _productTypeMapping?.LegacyId,
                PurchaseCurrencyPrice = _reject.PurchaseCurrencyPrice,
                OutgoingWarranty = _reject.OutgoingWarranty,
                DepartureDate = _reject.DepartureDate,
                PurchaseId = _reject.PurchaseId.HasValue ? _purchaseMapping?.LegacyId : _reject.PurchaseSqlId,
                SupplierDescription = _reject.SupplierDescription,
                ReturnDate = _reject.ReturnDate,
                ReturnType = _reject.ReturnType,
                SupplierProductTitle = _supplierProductMapping != null 
                    ? await GetProductMark(_supplierProductMapping.LegacyId)
                    : "",
                SupplierSerialNumber = _reject.SupplierSerialNumber,
                SupplierProductId = _supplierProductMapping?.LegacyId,
                SupplierProductType = _supplierProductMapping != null 
                    ? await GetProductType(_supplierProductMapping.LegacyId) 
                    : null
            });
        }

        private async Task<string> GetProductMark(int productId)
        {
            var sqlQuery = @"select [Марка] from [dbo].[Товары]
                            where[КодТовара]=@ProductId";
            var mark = await _db.QueryFirstOrDefaultAsync<string>(sqlQuery, new
            {
                ProductId = productId
            });

            return string.IsNullOrWhiteSpace(mark) ? "" : mark;
        }

        private async Task<string> GetProductType(int productId)
        {
            var sqlQuery = @"select [КодТипа] from [dbo].[Товары]
                            where[КодТовара]=@ProductId";
            var typeId = await _db.QueryFirstOrDefaultAsync<string>(sqlQuery, new
            {
                ProductId = productId
            });

            return typeId;
        }
    }
}
