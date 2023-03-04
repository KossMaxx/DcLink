using LegacySql.Domain.Clients;
using LegacySql.Domain.Firms;
using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacySql.Legacy.Data.Clients
{
    internal class ClientMapper
    {
        private readonly IDictionary<int, Guid?> _clientMap;
        private readonly IDictionary<int, Guid?> _managerMap;
        private readonly IDictionary<int, Guid?> _marketSegmentMap;
        private readonly IDictionary<int, Guid?> _warehouseMap;
        private readonly IDictionary<int, Guid?> _departmentMap;
        private readonly IDictionary<int, Guid?> _productGroupMap;
        private readonly IDictionary<int, Guid?> _activityTypeMap;
        private readonly IDictionary<int, Guid?> _segmentationTurnoverMap;

        public ClientMapper(
            IDictionary<int, Guid?> clientMap,
            IDictionary<int, Guid?> managerMap,
            IDictionary<int, Guid?> marketSegmentMap,
            IDictionary<int, Guid?> warehouseMap,
            IDictionary<int, Guid?> departmentMap,
            IDictionary<int, Guid?> productGroupMap,
            IDictionary<int, Guid?> activityTypeMap, 
            IDictionary<int, Guid?> segmentationTurnoverMap)
        {
            _clientMap = clientMap;
            _managerMap = managerMap;
            _marketSegmentMap = marketSegmentMap;
            _warehouseMap = warehouseMap;
            _departmentMap = departmentMap;
            _productGroupMap = productGroupMap;
            _activityTypeMap = activityTypeMap;
            _segmentationTurnoverMap = segmentationTurnoverMap;
        }

        public Client Map(
            ClientData master, 
            IEnumerable<ClientData> items, 
            IEnumerable<ClientProductsGroupsData> productsGroups = null,
            IEnumerable<ClientActivityTypesData> activityTypes = null)
        {
            var hasMap = _clientMap.ContainsKey(master.Client_Id);
            var client = new Client(hasMap)
            {
                Id = new IdMap(master.Client_Id, hasMap ? _clientMap[master.Client_Id] : null),
                Title = master.Client_Title,
                OnlySuperReports = master.Client_OnlySuperReports.HasValue ? master.Client_OnlySuperReports.Value : false,
                IsSupplier = master.Client_IsSupplier,
                IsCustomer = master.Client_IsCustomer,
                IsCompetitor = master.Client_IsCompetitor,
                Email = master.Client_Email,
                BalanceCurrencyId = master.Client_BalanceCurrencyId,
                CreditDays = master.Client_CreditDays,
                PriceValidDays = master.Client_PriceValidDays,
                Credit = master.Client_Credit,
                SurchargePercents = master.Client_SurchargePercents,
                BonusPercents = master.Client_BonusPercents,
                DelayOk = master.Client_DelayOk,
                Address = master.Client_Address,
                ChangedAt = master.Client_ChangedAt,
                City = master.Client_City?.ToString(),
                Consig = master.Client_Consig,
                ContactPerson = master.Client_ContactPerson,
                ContactPersonPhone = master.Client_ContactPersonPhone,
                DefaultPriceColumn = master.Client_DefaultPriceColumn,
                DeliveryTel = master.Client_DeliveryTel,
                IsPcAssembler = master.Client_IsPcAssembler,
                MobilePhone = master.Client_MobilePhone,
                SegmentAccessories = master.Client_SegmentAccessories.HasValue ? master.Client_SegmentAccessories.Value : false,
                SegmentActiveNet = master.Client_SegmentActiveNet.HasValue ? master.Client_SegmentActiveNet.Value : false,
                SegmentAv = master.Client_SegmentAv.HasValue ? master.Client_SegmentAv.Value : false,
                SegmentComponentsPc = master.Client_SegmentComponentsPc.HasValue ? master.Client_SegmentComponentsPc.Value : false,
                SegmentExpendables = master.Client_SegmentExpendables.HasValue ? master.Client_SegmentExpendables.Value : false,
                SegmentKbt = master.Client_SegmentKbt.HasValue ? master.Client_SegmentKbt.Value : false,
                SegmentMbt = master.Client_SegmentMbt.HasValue ? master.Client_SegmentMbt.Value : false,
                SegmentMobile = master.Client_SegmentMobile.HasValue ? master.Client_SegmentMobile.Value : false,
                SegmentNetSpecifility = master.Client_SegmentNetSpecifility,
                SegmentNotebooks = master.Client_SegmentNotebooks.HasValue ? master.Client_SegmentNotebooks.Value : false,
                SegmentPassiveNet = master.Client_SegmentPassiveNet.HasValue ? master.Client_SegmentPassiveNet.Value : false,
                SegmentPeriphery = master.Client_SegmentPeriphery.HasValue ? master.Client_SegmentPeriphery.Value : false,
                SegmentPrint = master.Client_SegmentPrint.HasValue ? master.Client_SegmentPrint.Value : false,
                SegmentReadyPc = master.Client_SegmentReadyPc.HasValue ? master.Client_SegmentReadyPc.Value : false,
                Website = master.Client_Website,
                RegionId = master.Client_RegionId,
                RegionTitle = master.Client_RegionTitle,
                Bonus = master.Client_Bonus,
                Penya = master.Client_Penya,
                ScContactEmail = master.Client_Sc_ContactEmail,
                ScContactPerson = master.Client_Sc_ContactPerson,
                ScContactPhone = master.Client_Sc_ContactPhone,
                ScDeliveryAddress = master.Client_Sc_DeliveryAddress,
                ScDeliveryPhone = master.Client_Sc_DeliveryPhone,
                ScDeliveryRecipient = master.Client_Sc_DeliveryRecipient,
                MainManagerId = master.Client_MainManagerId.HasValue
                                        ? new IdMap(
                                            master.Client_MainManagerId.Value,
                                            _managerMap.ContainsKey(master.Client_MainManagerId.Value)
                                            ? _managerMap[master.Client_MainManagerId.Value] : null)
                                        : null,
                ResponsibleManagerId = master.Client_ResponsibleManagerId.HasValue
                                               ? new IdMap(
                                                   master.Client_ResponsibleManagerId.Value,
                                                   _managerMap.ContainsKey(master.Client_ResponsibleManagerId.Value)
                                                   ? _managerMap[master.Client_ResponsibleManagerId.Value] : null)
                                               : null,
                DepartmentId = master.Client_Department.HasValue
                                       ? new IdMap(
                                           master.Client_Department.Value,
                                           _departmentMap.ContainsKey(master.Client_Department.Value)
                                           ? _departmentMap[master.Client_Department.Value] : null)
                                       : null,
                MarketSegmentId = master.Client_MarketSegmentId.HasValue
                                          ? new IdMap(
                                              master.Client_MarketSegmentId.Value,
                                              _marketSegmentMap.ContainsKey(master.Client_MarketSegmentId.Value)
                                              ? _marketSegmentMap[master.Client_MarketSegmentId.Value] : null)
                                          : null,
                MarketSegmentationTurnoverId = master.Client_MarketSegmentationTurnover.HasValue
                                          ? new IdMap(
                                              master.Client_MarketSegmentationTurnover.Value,
                                              _segmentationTurnoverMap.ContainsKey(master.Client_MarketSegmentationTurnover.Value)
                                              ? _segmentationTurnoverMap[master.Client_MarketSegmentationTurnover.Value] : null)
                                          : null
            };

            var firms = new Dictionary<int, FirmData>();
            var addresses = new Dictionary<int, DeliveryAddressData>();
            var priorities = new Dictionary<int, WarehousePriorityData>();
            var accesses = new Dictionary<int, WarehouseAccessData>();
            foreach (var clientDataItem in items)
            {
                if (clientDataItem.Firm != null)
                {
                    firms.TryAdd(clientDataItem.Firm.Firms_Id, clientDataItem.Firm);
                }
                if (clientDataItem.Address != null)
                {
                    addresses.TryAdd(clientDataItem.Address.ClientDeliveryAddress_Id, clientDataItem.Address);
                }
                if (clientDataItem.WarehousePriority != null)
                {
                    priorities.TryAdd(clientDataItem.WarehousePriority.ClientWarehousePriority_Id, clientDataItem.WarehousePriority);
                }
                if (clientDataItem.WarehouseAccess != null)
                {
                    accesses.TryAdd(clientDataItem.WarehouseAccess.ClientWarehouseAccess_Id, clientDataItem.WarehouseAccess);
                }
            }

            client.Firms = firms.Values.Select(f => new Firm
            {
                //TODO: Зачем нужен маппинг firm если подставляется guid партнера/соглашения?
                Id = new IdMap(f.Firms_Id, hasMap ? _clientMap[master.Client_Id] : null),
                Account = f.Firms_Account,
                Address = f.Firms_Address,
                BankCode = f.Firms_BankCode,
                BankName = f.Firms_BankName,
                LegalAddress = f.Firms_LegalAddress,
                Phone = f.Firms_Phone,
                TaxCode = f.Firms_TaxCode,
                IsNotResident = f.Firms_IsNotResident,
                Title = f.Firms_Title,
                PayerCode = f.Firms_PayerCode,
                CertificateNumber = f.Firms_CertificateNumber,
                NotVat = f.NotVat
            }).ToList();

            client.DeliveryAddresses = addresses.Values.Select(a => new ClientDeliveryAddress
            {
                Id = a.ClientDeliveryAddress_Id,
                Address = a.ClientDeliveryAddress_Address,
                ContactPerson = a.ClientDeliveryAddress_ContactPerson,
                Phone = a.ClientDeliveryAddress_Phone,
                Type = a.ClientDeliveryAddress_Type,
                WaybillAddress = a.ClientDeliveryAddress_WaybillAddress
            }).ToList();

            client.WarehousePriorities = priorities.Values.Select(p => new ClientWarehousePriority
            {
                Id = p.ClientWarehousePriority_Id,
                WarehouseId = new IdMap(
                                p.ClientWarehousePriority_WarehouseId,
                                _warehouseMap.ContainsKey(p.ClientWarehousePriority_WarehouseId)
                                ? _warehouseMap[p.ClientWarehousePriority_WarehouseId] : null),
                Priority = p.ClientWarehousePriority_Priority
            }).ToList();

            client.WarehouseAccesses = accesses.Values.Select(a => new ClientWarehouseAccess
            {
                Id = a.ClientWarehouseAccess_Id,
                WarehouseId = new IdMap(
                                a.ClientWarehouseAccess_ClientId,
                                _warehouseMap.ContainsKey(a.ClientWarehouseAccess_ClientId)
                                ? _warehouseMap[a.ClientWarehouseAccess_ClientId] : null),
                HasAccess = a.ClientWarehouseAccess_HasAccess
            }).ToList();

            if(productsGroups != null)
            {
                client.ClientProductGroups = productsGroups.Select(p => new ClientProductGroup
                {
                    Id = new IdMap(
                    p.SegmentationProductGroups_Id,
                    _productGroupMap.ContainsKey(p.SegmentationProductGroups_Id) ? _productGroupMap[p.SegmentationProductGroups_Id] : null),
                    Title = p.SegmentationProductGroups_Title
                });
            }

            if (activityTypes != null)
            {
                client.ClientActivityTypes = activityTypes.Select(p => new ClientActivityType
                {
                    Id = new IdMap(
                    p.SegmentationActivityType_Id,
                    _activityTypeMap.ContainsKey(p.SegmentationActivityType_Id) ? _activityTypeMap[p.SegmentationActivityType_Id] : null),
                    Title = p.SegmentationActivityType_Title
                });
            }

            return client;
        }
    }
}
