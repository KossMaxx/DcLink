using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_Clients_Shipping_Addr")]
    public class ClientDeliveryAddressEF
    {
        public int Id { get; set; }//shipping_addr_ID
        public string Address { get; set; }//dostavkaAdr
        public string ContactPerson { get; set; }//dostavkaFIO
        public string Phone { get; set; }//dostavkaTel
        public string WaybillAddress { get; set; }//WayBIll_addr
        public int Type { get; set; }//addr_type
        public int ClientId { get; set; }//client_ID
        public ClientEF Client { get; set; }
    }
}
