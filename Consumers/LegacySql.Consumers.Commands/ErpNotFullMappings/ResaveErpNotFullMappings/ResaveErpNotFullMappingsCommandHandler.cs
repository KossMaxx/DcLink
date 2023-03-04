using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.BankPayments.Import;
using MessageBus.Bonuses.Import;
using MessageBus.Cashboxes.Import;
using MessageBus.ClientOrder.Import;
using MessageBus.PriceConditions.Import;
using MessageBus.ProductPriceConditions.Import;
using MessageBus.Clients.Import;
using MessageBus.Departments.Import;
using MessageBus.Penalties.Import;
using MessageBus.ProductRefunds.Import;
using MessageBus.Products.Import;
using MessageBus.ProductSubtypes.Import;
using MessageBus.ProductTypes.Import;
using MessageBus.Purchases.Import;
using MessageBus.Rejects.Import;
using MessageBus.SellingPrices.Import;
using MessageBus.SupplierCurrencyRates.Import;
using MessageBus.WarehouseStocks.Import;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MessageBus.MovementOrders.Import;
using MessageBus.ProductMovings.Import;
using MessageBus.Waybills.Import;

namespace LegacySql.Consumers.Commands.ErpNotFullMappings.ResaveErpNotFullMappings
{
    public class ResaveErpNotFullMappingsCommandHandler : IRequestHandler<ResaveErpNotFullMappingsCommand>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IMediator _mediator;
        protected readonly ILogger<ResaveErpNotFullMappingsCommandHandler> _logger;

        public ResaveErpNotFullMappingsCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            IMediator mediator, 
            ILogger<ResaveErpNotFullMappingsCommandHandler> logger)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ResaveErpNotFullMappingsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Resave started");
            try
            {
                var allMappings = (await _erpNotFullMappedRepository.GetAllAsync()).ToList();

                await InvokeProductTypeEvent(allMappings.Where(e => e.Type == MappingTypes.ProductType), cancellationToken);
                await InvokeProductEvent(allMappings.Where(e => e.Type == MappingTypes.Product), cancellationToken);
                await InvokeClientOrderEvent(allMappings.Where(e => e.Type == MappingTypes.ClientOrder), cancellationToken);
                await InvokeClientOrderSerialNumberEvent(allMappings.Where(e => e.Type == MappingTypes.ClientOrderSerialNumbers), cancellationToken);
                await InvokeSellingPriceEvent(allMappings.Where(e => e.Type == MappingTypes.SellingPrice), cancellationToken);
                await InvokePriceConditionEvent(allMappings.Where(e => e.Type == MappingTypes.PriceCondition), cancellationToken);
                await InvokeProductPriceConditionEvent(allMappings.Where(e => e.Type == MappingTypes.ProductPriceCondition), cancellationToken);
                await InvokeBankPaymentEvent(allMappings.Where(e => e.Type == MappingTypes.BankPayment), cancellationToken);
                await InvokeClientEvent(allMappings.Where(e => e.Type == MappingTypes.Client), cancellationToken);
                await InvokeCashboxPaymentEvent(allMappings.Where(e => e.Type == MappingTypes.CashboxPayment), cancellationToken);
                await InvokeWarehouseStockEvent(allMappings.Where(e => e.Type == MappingTypes.WarehouseStock), cancellationToken);
                await InvokeProductSubtypesEvent(allMappings.Where(e => e.Type == MappingTypes.ProductSubtype), cancellationToken);
                await InvokePenaltyEvent(allMappings.Where(e => e.Type == MappingTypes.Penalty), cancellationToken);
                await InvokeBonusEvent(allMappings.Where(e => e.Type == MappingTypes.Bonus), cancellationToken);
                await InvokeSupplierCurrensyRateEvent(allMappings.Where(e => e.Type == MappingTypes.SupplierCurrencyRate), cancellationToken);
                await InvokePurchaseEvent(allMappings.Where(e => e.Type == MappingTypes.Purchase), cancellationToken);
                await InvokeProductRefundEvent(allMappings.Where(e => e.Type == MappingTypes.ProductRefund), cancellationToken);
                await InvokeClientOrderDeliveryEvent(allMappings.Where(e => e.Type == MappingTypes.ClientOrderDelivery), cancellationToken);
                await InvokeRejectEvent(allMappings.Where(e => e.Type == MappingTypes.Reject), cancellationToken);
                await InvokeRejectReplacementCostEvent(allMappings.Where(e => e.Type == MappingTypes.RejectReplacementCost), cancellationToken);
                await InvokeClientFirmEvent(allMappings.Where(e => e.Type == MappingTypes.ClientFirm), cancellationToken);
                await InvokePartnerEvent(allMappings.Where(e => e.Type == MappingTypes.Partner), cancellationToken);
                await InvokeDepartmentEvent(allMappings.Where(e => e.Type == MappingTypes.Department), cancellationToken);
                await InvokePaymentOrderEvent(allMappings.Where(e => e.Type == MappingTypes.PaymentOrder), cancellationToken);
                await InvokeMovementOrderEvent(allMappings.Where(e => e.Type == MappingTypes.MovementOrder), cancellationToken);
                await InvokeProductMovingEvent(allMappings.Where(e => e.Type == MappingTypes.ProductMoving), cancellationToken);
                await InvokeWaybillEvent(allMappings.Where(e => e.Type == MappingTypes.Waybill), cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка пересохранения объекта\n{e}");
            }

            _logger.LogInformation("Resave finished");

            return new Unit();
        }

        private async Task InvokeBankPaymentEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpBankPaymentEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpBankPaymentDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeClientEvent(IEnumerable<ErpNotFullMapped> clients, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpClientEvent(
                    clients.Select(e => JsonConvert.DeserializeObject<ErpClientDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeClientOrderSerialNumberEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpClientOrderSerialNumbersEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpClientOrderSerialNumbersDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeProductTypeEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpProductTypeEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ProductTypeErpDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeProductEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpProductEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpProductDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeClientOrderEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpClientOrderEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpClientOrderDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeSellingPriceEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            var items = messages.Select(e => JsonConvert.DeserializeObject<ErpSellingPriceDto>(e.Value)).ToList();
            await _mediator.Publish(new ResaveErpSellingPriceEvent(items), cancellationToken);
        }

        private async Task InvokePriceConditionEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            var items = messages.Select(e => JsonConvert.DeserializeObject<ErpPriceConditionDto>(e.Value)).ToList();
            await _mediator.Publish(new ResaveErpPriceConditionEvent(items), cancellationToken);
        }

        private async Task InvokeProductPriceConditionEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            var items = messages.Select(e => JsonConvert.DeserializeObject<ErpProductPriceConditionDto>(e.Value)).ToList();
            await _mediator.Publish(new ResaveErpProductPriceConditionEvent(items), cancellationToken);
        }

        private async Task InvokeCashboxPaymentEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpCashboxPaymentEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpCashboxPaymentDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeWarehouseStockEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpWarehouseStockEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpWarehouseStockDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeProductSubtypesEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpProductSubtypeEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpProductSubtypeDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokePenaltyEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            var items = messages.Select(e => JsonConvert.DeserializeObject<ErpPenaltyDto>(e.Value)).ToList();
            await _mediator.Publish(new ResaveErpPenaltyEvent(items), cancellationToken);
        }

        private async Task InvokeBonusEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            var items = messages.Select(e => JsonConvert.DeserializeObject<ErpBonusDto>(e.Value)).ToList();
            await _mediator.Publish(new ResaveErpBonusEvent(items), cancellationToken);
        }

        private async Task InvokeSupplierCurrensyRateEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            var items = messages.Select(e => JsonConvert.DeserializeObject<ErpSupplierCurrencyRateDto>(e.Value)).ToList();
            await _mediator.Publish(new ResaveErpSupplierCurrencyRateEvent(items), cancellationToken);
        }

        private async Task InvokePurchaseEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpPurchaseEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpPurchaseDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }
        private async Task InvokeProductRefundEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpProductRefundEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpProductRefundDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeClientOrderDeliveryEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpClientOrderDeliveryEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpClientOrderDeliveryDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeRejectEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpRejectEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpRejectDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeRejectReplacementCostEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpRejectReplacementCostEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpRejectReplacementCostDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }
        private async Task InvokeClientFirmEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpClientFirmEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpFirmDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }
        private async Task InvokePartnerEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpPartnerEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpPartnerDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeDepartmentEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpDepartmentEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpDepartmentDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokePaymentOrderEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpPaymentOrderEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpPaymentOrderDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeMovementOrderEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpMovementOrderEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpMovementOrderDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeProductMovingEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpProductMovingEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpProductMovingDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }

        private async Task InvokeWaybillEvent(IEnumerable<ErpNotFullMapped> messages, CancellationToken cancellationToken)
        {
            await _mediator.Publish(
                new ResaveErpWaybillEvent(
                    messages.Select(e => JsonConvert.DeserializeObject<ErpWaybillDto>(e.Value))
                        .ToList()),
                cancellationToken);
        }
    }
}