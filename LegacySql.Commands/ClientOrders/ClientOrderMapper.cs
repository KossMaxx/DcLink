using LegacySql.Domain.ClientOrders;
using MessageBus.ClientOrder.Export;
using System.Linq;

namespace LegacySql.Commands.ClientOrders
{
    public class ClientOrderMapper
    {
        internal ClientOrderDto MapToDto(ClientOrder order)
        {
            return new ClientOrderDto
            {
                Number = order.Id.InnerId,
                Date = order.Date,
                ClientId = order.ClientId.ExternalId,
                PaymentType = order.PaymentType,
                Comments = order.Comments,
                MarketplaceNumber = order.MarketplaceNumber,
                RecipientOKPO = order.RecipientOKPO,
                Source = order.Source != null
                    ? new DepartmentDto
                    {
                        Title = order.Source.Title,
                        Type = order.Source.Type.ToString()
                    }
                    : null,
                Items = order.Items.Select(i => new ClientOrderItemDto
                {
                    NomenclatureId = i.NomenclatureId.ExternalId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    PriceUAH = i.PriceUAH,
                    Warranty = i.Warranty,
                    SerialNumbers = i.SerialNumbers
                }),
                Delivery = order.Delivery != null
                    ? new OrderDeliveryDto
                    {
                        Method = order.Delivery.Method != null
                            ? new OrderDeliveryMethodDto
                            {
                                Carrier = new OrderDeliveryMethodCarrierDto
                                {
                                    Id = order.Delivery.Method.Carrier.Id,
                                    Title = order.Delivery.Method.Carrier.Title
                                },
                                Type = new OrderDeliveryMethodTypeDto
                                {
                                    Id = order.Delivery.Method.Type.Id,
                                    Title = order.Delivery.Method.Type.Title
                                }
                            }
                            : null,
                        Recipient = new OrderDeliveryRecipientDto
                        {
                            Address = new OrderDeliveryRecipientAddressDto
                            {
                                City = order.Delivery.Recipient.Address.City,
                                Title = order.Delivery.Recipient.Address.Title,
                                CityId = order.Delivery.Recipient.Address.CityId,
                            },
                            Name = order.Delivery.Recipient.Name,
                            Phone = order.Delivery.Recipient.Phone,
                            Email = order.Delivery.Recipient.Email,
                        },
                        Weight = order.Delivery.Weight,
                        Volume = order.Delivery.Volume,
                        DeclaredPrice = order.Delivery.DeclaredPrice,
                        PayerType = order.Delivery.PayerType,
                        PaymentMethod = order.Delivery.PaymentMethod,
                        CargoType = order.Delivery.CargoType,
                        ServiceType = order.Delivery.ServiceType,
                        CashOnDelivery = order.Delivery.CashOnDelivery,
                        Warehouse = order.Delivery.Warehouse == null
                            ? null
                            : new OrderDeliveryWarehouseDto
                            {
                                Id = order.Delivery.Warehouse.Id,
                                Number = order.Delivery.Warehouse.Number,
                            },
                        CargoInvoice = order.Delivery.CargoInvoice
                    }
                    : null,
                IsExecuted = order.IsExecuted,
                IsPaid = order.IsPaid,
                WarehouseId = order.WarehouseId?.ExternalId,
                ManagerId = order.ManagerId?.ExternalId,
                Quantity = order.Quantity,
                Amount = order.Amount,
                PaymentDate = order.PaymentDate,
                BillNumber = order.BillNumber,
                FirmSqlId = order.FirmSqlId
            };
        }
    }
}
