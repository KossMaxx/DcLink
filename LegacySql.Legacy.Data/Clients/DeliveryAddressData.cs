using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Legacy.Data.Clients
{
    internal class DeliveryAddressData
    {
        public int ClientDeliveryAddress_Id { get; set; }
        public string ClientDeliveryAddress_Address { get; set; }
        public string ClientDeliveryAddress_ContactPerson { get; set; }
        public string ClientDeliveryAddress_Phone { get; set; }
        public string ClientDeliveryAddress_WaybillAddress { get; set; }
        public int ClientDeliveryAddress_Type { get; set; }
        public int ClientDeliveryAddress_ClientId { get; set; }
    }
}
