using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<DeliveryEF>
    {
        public void Configure(EntityTypeBuilder<DeliveryEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.TypeId).HasColumnName("gorod");
            builder.Property(e => e.RecipientName).HasColumnName("poluchatel");
            builder.Property(e => e.RecipientPhone).HasColumnName("tel").HasMaxLength(50);
            builder.Property(e => e.RecipientAddress).HasColumnName("kuda").HasMaxLength(200);
            builder.Property(e => e.RecipientCityId).HasColumnName("CityRecipient");
            builder.Property(e => e.Weight).HasColumnName("Weight");
            builder.Property(e => e.Volume).HasColumnName("Vol");
            builder.Property(e => e.DeclaredPrice).HasColumnName("DeclaredPrice").HasColumnType("money");
            builder.Property(e => e.PayerType).HasColumnName("PayerType").HasMaxLength(50);
            builder.Property(e => e.PaymentMethod).HasColumnName("PaymentMethod").HasMaxLength(50);
            builder.Property(e => e.CargoType).HasColumnName("CargoType").HasMaxLength(50);
            builder.Property(e => e.ServiceType).HasColumnName("ServiceType").HasMaxLength(50);
            builder.Property(e => e.CashOnDelivery).HasColumnName("CashOnDelivery").HasMaxLength(50);
            builder.Property(e => e.RecipientEmail).HasColumnName("email").HasMaxLength(100);
            builder.Property(e => e.CargoInvoice).HasColumnName("CargoInvoice").HasMaxLength(50).HasComment("Квитанция транспортной службы");
            builder.Property(e => e.ChangedAt).HasColumnName("modified_at").HasColumnType("datetime"); 

            #region AutoGeneretedConfiguration

            //entity.ToTable("dostavka");

            //entity.HasIndex(e => new { e.Id, e.Gorod })
            //    .HasName("<Name of Missing Index, sysname,>");

            //entity.HasIndex(e => new { e.Id, e.Zadacha, e.KlientId, e.Vipolneno, e.StatusDc })
            //    .HasName("dostavka_vipolneno");

            //entity.Property(e => e.Boxes).HasColumnName("boxes");

            //entity.Property(e => e.CargoType).HasMaxLength(50);

            //entity.Property(e => e.Confirmed).HasColumnName("confirmed");

            //entity.Property(e => e.Cost)
            //    .HasColumnName("cost")
            //    .HasColumnType("money");

            //entity.Property(e => e.Ddd)
            //    .HasColumnName("ddd")
            //    .HasColumnType("datetime");

            //entity.Property(e => e.DeclaredPrice).HasColumnType("money");

            //entity.Property(e => e.DocumentsGet).HasColumnName("documents_get");

            //entity.Property(e => e.DocumentsGive).HasColumnName("documents_give");

            //entity.Property(e => e.Email)
            //    .HasColumnName("email")
            //    .HasMaxLength(100);

            //entity.Property(e => e.EstimatedDeliveryDate).HasColumnType("date");

            //entity.Property(e => e.FinDate)
            //    .HasColumnName("finDate")
            //    .HasColumnType("smalldatetime");

            //entity.Property(e => e.FinUser)
            //    .HasColumnName("finUser")
            //    .HasMaxLength(20);

            //entity.Property(e => e.Finansi).HasColumnName("finansi");

            //entity.Property(e => e.Finished).HasColumnName("finished");

            //entity.Property(e => e.Fio)
            //    .HasColumnName("FIO")
            //    .HasMaxLength(50);

            //entity.Property(e => e.Justinid).HasColumnName("justinid");

            //entity.Property(e => e.Klient)
            //    .HasColumnName("klient")
            //    .HasMaxLength(50);

            //entity.Property(e => e.KlientId).HasColumnName("klientID");

            //entity.Property(e => e.Kogda)
            //    .HasColumnName("kogda")
            //    .HasMaxLength(50);

            //entity.Property(e => e.Kurs)
            //    .HasColumnName("kurs")
            //    .HasColumnType("smallmoney");

            //entity.Property(e => e.MarketplaceOrderStatus)
            //    .HasColumnName("marketplace_order_status")
            //    .HasComment("Last setted status to API");

            //entity.Property(e => e.Nal).HasColumnName("nal");

            //entity.Property(e => e.NpAddrId).HasColumnName("np_addr_ID");

            //entity.Property(e => e.NpAddrRef).HasColumnName("np_addr_Ref");

            //entity.Property(e => e.NpCostOnSite)
            //    .HasColumnName("NP_CostOnSite")
            //    .HasColumnType("money");

            //entity.Property(e => e.NpCounterparty)
            //    .HasColumnName("NP_Counterparty")
            //    .HasMaxLength(50);

            //entity.Property(e => e.NpRef)
            //    .HasColumnName("NP_Ref")
            //    .HasMaxLength(50);

            //entity.Property(e => e.NpSenderId).HasColumnName("NP_senderID");

            //entity.Property(e => e.Ooo).HasColumnName("OOO");

            //entity.Property(e => e.Oplacheno).HasColumnName("oplacheno");

            //entity.Property(e => e.PayerType).HasMaxLength(50);

            //entity.Property(e => e.PaymentControl).HasColumnName("payment_control");

            //entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            //entity.Property(e => e.Prinyato)
            //    .HasColumnName("prinyato")
            //    .HasMaxLength(50);

            //entity.Property(e => e.Query).HasColumnName("query");

            //entity.Property(e => e.RedeliveryCargoType).HasMaxLength(50);

            //entity.Property(e => e.RedeliveryPayerType).HasMaxLength(50);

            //entity.Property(e => e.RedeliveryString).HasMaxLength(50);

            //entity.Property(e => e.RoutePosition).HasColumnName("routePosition");

            //entity.Property(e => e.ServiceType).HasMaxLength(50);

            //entity.Property(e => e.ShippingAddrId).HasColumnName("shipping_addr_ID");

            //entity.Property(e => e.Sozd)
            //    .HasColumnName("sozd")
            //    .HasMaxLength(20);

            //entity.Property(e => e.StatusDc).HasColumnName("statusDC");

            //entity.Property(e => e.StatusEn)
            //    .HasColumnName("statusEN")
            //    .HasMaxLength(255);

            //entity.Property(e => e.SumOfPaymentBeforePickup).HasColumnType("money");

            //entity.Property(e => e.TypeDocument).HasMaxLength(50);

            //entity.Property(e => e.VipDdd)
            //    .HasColumnName("vipDdd")
            //    .HasColumnType("smalldatetime");

            //entity.Property(e => e.VipUser)
            //    .HasColumnName("vipUser")
            //    .HasMaxLength(20);

            //entity.Property(e => e.Vipolneno).HasColumnName("vipolneno");

            //entity.Property(e => e.WayBillAddr).HasMaxLength(100);

            //entity.Property(e => e.Z).HasColumnName("z");

            //entity.Property(e => e.Zadacha)
            //    .HasColumnName("zadacha")
            //    .HasMaxLength(200);

            #endregion
        }
    }
}
