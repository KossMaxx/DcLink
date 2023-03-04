using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<ClientEF>
    {
        public void Configure(EntityTypeBuilder<ClientEF> builder)
        {
            builder.HasKey(e => e.Id).HasName("КодПоставщика");

            builder.HasIndex(b => b.Title).IsUnique();

            builder.Property(e => e.Id).HasColumnName("КодПоставщика");
            builder.Property(e => e.Title).HasColumnName("Название").HasMaxLength(50).IsRequired();
            builder.Property(e => e.OnlySuperReports).HasColumnName("H");
            builder.Property(e => e.IsSupplier).HasColumnName("Поставщик");
            builder.Property(e => e.IsCustomer).HasColumnName("Покупатель");
            builder.Property(e => e.IsCompetitor).HasColumnName("Konkurent");
            builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
            builder.Property(e => e.MasterId).HasColumnName("masterID");
            builder.HasIndex(e => e.MasterId).HasDatabaseName("<Клиенты_masterID>");
            builder.Property(e => e.ChangedAt)
                    .HasColumnName("modified_at")
                    .HasColumnType("datetime");
            builder.Property(e => e.BalanceCurrencyId).HasColumnName("ВалютаБаланса");
            builder.HasIndex(e => e.BalanceCurrencyId).HasDatabaseName("IX_Клиенты_Currency");

            builder.HasMany(c => c.Nested)
                .WithOne(c => c.Master).HasForeignKey(c => c.MasterId);

            builder.Property(e => e.Department).HasColumnName("department");
            builder.Property(e => e.IsTechnicalAccount).HasColumnName("price_log");
            builder.Property(e => e.CreditDays).HasColumnName("kredit");
            builder.Property(e => e.PriceValidDays).HasColumnName("PriceValidDays");

            builder.Property(e => e.MainManagerId).HasColumnName("manager1");
            builder.HasIndex(e => e.MainManagerId).HasDatabaseName("IX_Клиенты_Manager1");
            builder.Property(e => e.ResponsibleManagerId).HasColumnName("manager2");
            builder.HasIndex(e => e.ResponsibleManagerId).HasDatabaseName("IX_Клиенты_manager2");
            builder.Property(e => e.MarketSegmentId).HasColumnName("segmentation");

            builder.Property(e => e.Credit).HasColumnName("кредит").HasColumnType("money");
            builder.Property(e => e.SurchargePercents).HasColumnName("penyaV");
            builder.Property(e => e.BonusPercents).HasColumnName("bonusV");
            builder.Property(e => e.DelayOk).HasColumnName("delayOk");

            builder.Property(e => e.DeliveryTel)
                .HasColumnName("dostavkaTel")
                .HasMaxLength(50);

            builder.Property(e => e.SegmentAccessories)
                .HasColumnName("segment_accessories");
            builder.Property(e => e.SegmentActiveNet)
                .HasColumnName("segment_active_net");
            builder.Property(e => e.SegmentAv)
                .HasColumnName("segment_AV");
            builder.Property(e => e.SegmentComponentsPc)
                .HasColumnName("segment_componentsPC");
            builder.Property(e => e.SegmentExpendables)
                .HasColumnName("segment_expendables");
            builder.Property(e => e.SegmentKbt)
                .HasColumnName("segment_KBT");
            builder.Property(e => e.SegmentMbt)
                .HasColumnName("segment_MBT");
            builder.Property(e => e.SegmentMobile)
                .HasColumnName("segment_mobile");
            builder.Property(e => e.SegmentNotebooks)
                .HasColumnName("segment_notebooks");
            builder.Property(e => e.SegmentPassiveNet)
                .HasColumnName("segment_passive_net");
            builder.Property(e => e.SegmentPeriphery)
                .HasColumnName("segment_periphery");
            builder.Property(e => e.SegmentPrint)
                .HasColumnName("segment_print");
            builder.Property(e => e.SegmentReadyPc)
                .HasColumnName("segment_readyPC");
            builder.Property(e => e.Consig)
                .HasColumnName("consig");
            builder.Property(e => e.IsPcAssembler)
                .HasColumnName("is_PC_assembler");
            builder.Property(e => e.SegmentNetSpecifility)
                .HasColumnName("segment_net_specifility");

            builder.Property(e => e.Website)
                .HasColumnName("website")
                .HasMaxLength(150);

            builder.Property(e => e.ContactPersonPhone)
                .HasColumnName("НомерТелефона")
                .HasMaxLength(255);

            builder.Property(e => e.ContactPerson)
                .HasColumnName("ОбращатьсяК")
                .HasMaxLength(50);

            builder.Property(e => e.Address)
                .HasColumnName("Адрес")
                .HasMaxLength(500);

            builder.Property(e => e.MobilePhone)
                .HasColumnName("cell_ID")
                .HasMaxLength(50);

            builder.Property(e => e.DefaultPriceColumn).HasColumnName("колонка");
            builder.Property(e => e.City).HasColumnName("Город").HasMaxLength(50);

            #region AutoGeneretedConfiguration

            // entity.HasKey(e => e.КодПоставщика);
            //
            // entity.HasIndex(e => e.Manager1)
            //     .HasName("IX_Клиенты_Manager1");
            //
            // entity.HasIndex(e => e.Manager2);
            //
            // entity.HasIndex(e => e.RegionId)
            //     .HasName("IX_Клиенты_RegionID");
            //
            // entity.HasIndex(e => e.Название)
            //     .HasName("<КлиентыНазвание>")
            //     .IsUnique();
            //
            // entity.HasIndex(e => new { e.Blok, e.MasterId })
            //     .HasName("Клиенты_masterID");
            //
            // entity.Property(e => e.Balance)
            //     .HasColumnName("balance")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.BalanceDelay)
            //     .HasColumnName("balanceDelay")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.BalanceS)
            //     .HasColumnName("balanceS")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.Blok).HasColumnName("blok");
            //
            // entity.Property(e => e.BlokApi).HasColumnName("blokAPI");
            //
            // entity.Property(e => e.Bonus).HasColumnName("bonus");
            //
            // entity.Property(e => e.BonusManager).HasColumnName("bonus_manager");
            //
            // entity.Property(e => e.BonusManagerV).HasColumnName("bonus_manager_V");
            //
            // entity.Property(e => e.BonusQ)
            //     .HasColumnName("bonusQ")
            //     .HasColumnType("smallmoney");
            //
            // entity.Property(e => e.BonusV).HasColumnName("bonusV");
            //
            // entity.Property(e => e.BrokerId).HasColumnName("brokerID");
            //
            // entity.Property(e => e.CellId)
            //     .HasColumnName("cell_ID")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Consig).HasColumnName("consig");
            //
            // entity.Property(e => e.DataLastSale).HasColumnType("datetime");
            //
            // entity.Property(e => e.DataRndogovor)
            //     .HasColumnName("dataRNdogovor")
            //     .HasColumnType("datetime");
            //
            // entity.Property(e => e.Datalastimport)
            //     .HasColumnName("datalastimport")
            //     .HasColumnType("datetime");
            //
            // entity.Property(e => e.DelayOk).HasColumnName("delayOK");
            //
            // entity.Property(e => e.DocBarcodeUrl)
            //     .HasColumnName("doc_barcode_url")
            //     .HasMaxLength(150);
            //
            // entity.Property(e => e.DocumentsDeliveryPrim).HasMaxLength(50);
            //
            // entity.Property(e => e.DogovorFilename).HasMaxLength(150);
            //
            // entity.Property(e => e.DostavkaAdr)
            //     .HasColumnName("dostavkaAdr")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.DostavkaFio)
            //     .HasColumnName("dostavkaFIO")
            //     .HasMaxLength(150);
            //
            // entity.Property(e => e.DostavkaTel)
            //     .HasColumnName("dostavkaTel")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Dropship).HasColumnName("dropship");
            //
            // entity.Property(e => e.Email)
            //     .HasColumnName("email")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.EmailBuh)
            //     .HasColumnName("emailBuh")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.EmailFin)
            //     .HasColumnName("emailFin")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.EmailSklad)
            //     .HasColumnName("emailSklad")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.FffQ)
            //     .HasColumnName("fffQ")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.FinTransfer).HasColumnName("fin_transfer");
            //
            // entity.Property(e => e.FirstStr).HasColumnName("first_str");
            //
            // entity.Property(e => e.H).HasColumnName("h");
            //
            // entity.Property(e => e.HtmlcolumnNo).HasColumnName("HTMLColumnNo");
            //
            // entity.Property(e => e.Htmllink).HasColumnName("HTMLlink");
            //
            // entity.Property(e => e.Htmlstr)
            //     .HasColumnName("HTMLstr")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Icq)
            //     .HasColumnName("ICQ")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Income).HasColumnName("income");
            //
            // entity.Property(e => e.Ip)
            //     .HasColumnName("IP")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Iplock).HasColumnName("IPlock");
            //
            // entity.Property(e => e.IsBuyAtWarehouse).HasColumnName("isBuyAtWarehouse");
            //
            // entity.Property(e => e.Konkurent).HasColumnName("konkurent");
            //
            // entity.Property(e => e.Kredit).HasColumnName("kredit");
            //
            // entity.Property(e => e.LastStr).HasColumnName("last_str");
            //
            // entity.Property(e => e.Manager1).HasColumnName("manager1");
            //
            // entity.Property(e => e.Manager2).HasColumnName("manager2");
            //
            // entity.Property(e => e.MasterId).HasColumnName("masterID");
            //
            // entity.Property(e => e.MoneyCredit)
            //     .HasColumnName("money_credit")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.MoneySsFin)
            //     .HasColumnName("money_SS_fin")
            //     .HasColumnType("decimal(5, 4)");
            //
            // entity.Property(e => e.MoneySsFinCompensation)
            //     .HasColumnName("money_SS_fin_compensation")
            //     .HasColumnType("decimal(5, 4)");
            //
            // entity.Property(e => e.NalColumn).HasColumnName("nalColumn");
            //
            // entity.Property(e => e.New).HasColumnName("new");
            //
            // entity.Property(e => e.NewEndDate)
            //     .HasColumnName("newEndDate")
            //     .HasColumnType("datetime");
            //
            // entity.Property(e => e.Nickname)
            //     .HasColumnName("nickname")
            //     .HasMaxLength(30);
            //
            // entity.Property(e => e.Nickname2)
            //     .HasColumnName("nickname2")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.NoStock).HasMaxLength(50);
            //
            // entity.Property(e => e.NoTovar).HasMaxLength(50);
            //
            // entity.Property(e => e.Online).HasColumnName("online");
            //
            // entity.Property(e => e.Pass)
            //     .HasColumnName("pass")
            //     .HasMaxLength(20);
            //
            // entity.Property(e => e.Path)
            //     .HasColumnName("path")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.Path1)
            //     .HasColumnName("path1")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.PaymentControl).HasColumnName("payment_control");
            //
            // entity.Property(e => e.Penya).HasColumnName("penya");
            //
            // entity.Property(e => e.PenyaManager).HasColumnName("penya_manager");
            //
            // entity.Property(e => e.PenyaManagerV).HasColumnName("penya_manager_V");
            //
            // entity.Property(e => e.PenyaV).HasColumnName("penyaV");
            //
            // entity.Property(e => e.PlId).HasColumnName("PL_id");
            //
            // entity.Property(e => e.PlSubtypeId).HasColumnName("PL_subtype_id");
            //
            // entity.Property(e => e.PlanQ)
            //     .HasColumnName("planQ")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.PnFirm)
            //     .HasColumnName("pnFirm")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.PnName)
            //     .HasColumnName("pnName")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.PricesBnOnly).HasColumnName("prices_bn_only");
            //
            // entity.Property(e => e.Problem).HasColumnName("problem");
            //
            // entity.Property(e => e.Rassilka).HasColumnName("rassilka");
            //
            // entity.Property(e => e.Reason)
            //     .HasColumnName("reason")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.ReasonApi)
            //     .HasColumnName("reasonAPI")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.RegionId).HasColumnName("regionID");
            //
            // entity.Property(e => e.ReportAdress).HasMaxLength(50);
            //
            // entity.Property(e => e.ReportCity).HasMaxLength(20);
            //
            // entity.Property(e => e.ReportName).HasMaxLength(50);
            //
            // entity.Property(e => e.ReportZip).HasMaxLength(5);
            //
            // entity.Property(e => e.ReservDays).HasColumnName("reserv_days");
            //
            // entity.Property(e => e.ReservToDate)
            //     .HasColumnName("reserv_to_date")
            //     .HasColumnType("datetime");
            //
            // entity.Property(e => e.ResiduesControl).HasColumnName("residues_control");
            //
            // entity.Property(e => e.RmaBalanceId).HasColumnName("rma_balance_id");
            //
            // entity.Property(e => e.SegmentAccessories).HasColumnName("segment_accessories");
            //
            // entity.Property(e => e.SegmentActiveNet).HasColumnName("segment_active_net");
            //
            // entity.Property(e => e.SegmentAv).HasColumnName("segment_AV");
            //
            // entity.Property(e => e.SegmentComponentsPc).HasColumnName("segment_componentsPC");
            //
            // entity.Property(e => e.SegmentExpendables).HasColumnName("segment_expendables");
            //
            // entity.Property(e => e.SegmentKbt).HasColumnName("segment_KBT");
            //
            // entity.Property(e => e.SegmentMbt).HasColumnName("segment_MBT");
            //
            // entity.Property(e => e.SegmentMobile).HasColumnName("segment_mobile");
            //
            // entity.Property(e => e.SegmentNotebooks).HasColumnName("segment_notebooks");
            //
            // entity.Property(e => e.SegmentPassiveNet).HasColumnName("segment_passive_net");
            //
            // entity.Property(e => e.SegmentPeriphery).HasColumnName("segment_periphery");
            //
            // entity.Property(e => e.SegmentPrint).HasColumnName("segment_print");
            //
            // entity.Property(e => e.SegmentReadyPc).HasColumnName("segment_readyPC");
            //
            // entity.Property(e => e.SelfShipmentPermission).HasColumnName("self_shipment_permission");
            //
            // entity.Property(e => e.SendByEmail).HasColumnName("sendByEmail");
            //
            // entity.Property(e => e.Shownal).HasColumnName("shownal");
            //
            // entity.Property(e => e.Skype).HasMaxLength(50);
            //
            // entity.Property(e => e.Spec).HasColumnName("spec");
            //
            // entity.Property(e => e.StPos).HasColumnName("st_pos");
            //
            // entity.Property(e => e.StPrice).HasColumnName("st_price");
            //
            // entity.Property(e => e.StPriceRozn).HasColumnName("st_price_rozn");
            //
            // entity.Property(e => e.TechnicalDelayDisable)
            //     .HasColumnName("technical_delay_disable")
            //     .HasComment("отключает отображение дней отсрочки и кредитного лимита в В2В");
            //
            // entity.Property(e => e.TodayPayments)
            //     .HasColumnName("today_payments")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.Todel)
            //     .HasColumnName("todel")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.TomorowPayments)
            //     .HasColumnName("tomorow_payments")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.Urlportal)
            //     .HasColumnName("URLportal")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.Urlprice)
            //     .HasColumnName("URLprice")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.WarDefaultShiping).HasColumnName("war_DefaultShiping");
            //
            // entity.Property(e => e.WarDostavkaAdr)
            //     .HasColumnName("war_dostavkaAdr")
            //     .HasMaxLength(200);
            //
            // entity.Property(e => e.WarDostavkaFio)
            //     .HasColumnName("war_dostavkaFIO")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.WarDostavkaTel)
            //     .HasColumnName("war_dostavkaTel")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.WarFio)
            //     .HasColumnName("warFIO")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.WarManager).HasColumnName("war_manager");
            //
            // entity.Property(e => e.Warmail)
            //     .HasColumnName("warmail")
            //     .HasMaxLength(250);
            //
            // entity.Property(e => e.Wartel)
            //     .HasColumnName("wartel")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.WayBillAddr)
            //     .HasColumnName("WayBIll_addr")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.WebSkladDetails).HasColumnName("webSkladDetails");
            //
            // entity.Property(e => e.Webclose).HasColumnName("webclose");
            //
            // entity.Property(e => e.Website)
            //     .HasColumnName("website")
            //     .HasMaxLength(150);
            //
            // entity.Property(e => e.WithoutRefOnly).HasColumnName("without_ref_only");
            //
            // entity.Property(e => e.Wkurs)
            //     .HasColumnName("wkurs")
            //     .HasColumnType("smallmoney");
            //
            // entity.Property(e => e.Wlink)
            //     .HasColumnName("wlink")
            //     .HasMaxLength(150);
            //
            // entity.Property(e => e.Wlink2)
            //     .HasColumnName("wlink2")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.WlinkEnd)
            //     .HasColumnName("wlinkEND")
            //     .HasMaxLength(100);
            //
            // entity.Property(e => e.Woffset).HasColumnName("woffset");
            //
            // entity.Property(e => e.Woffset2).HasColumnName("woffset2");
            //
            // entity.Property(e => e.Wsearch)
            //     .HasColumnName("wsearch")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Wsearch2)
            //     .HasColumnName("wsearch2")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Xmlartikul).HasColumnName("XMLartikul");
            //
            // entity.Property(e => e.XmlprocedureName)
            //     .HasColumnName("XMLprocedureName")
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.Xmlwar).HasColumnName("XMLwar");
            //
            // entity.Property(e => e.Z).HasColumnName("z");
            //
            // entity.Property(e => e.ZakazDescr).HasMaxLength(50);
            //
            // entity.Property(e => e.Адрес).HasMaxLength(500);
            //
            // entity.Property(e => e.Валюта).HasColumnName("валюта");
            //
            //
            // entity.Property(e => e.ДоговорОтдан).HasColumnName("договор_отдан");
            //
            // entity.Property(e => e.ДоговорПолучен).HasColumnName("договор_получен");
            //
            // entity.Property(e => e.Интерес)
            //     .HasColumnName("интерес")
            //     .HasMaxLength(30);
            //
            // entity.Property(e => e.Колонка).HasColumnName("колонка");
            //
            // entity.Property(e => e.Кредит)
            //     .HasColumnName("кредит")
            //     .HasColumnType("money");
            //
            // entity.Property(e => e.Менеджер)
            //     .HasColumnName("менеджер")
            //     .HasMaxLength(20);
            //
            // entity.Property(e => e.Название)
            //     .IsRequired()
            //     .HasMaxLength(50);
            //
            // entity.Property(e => e.НомерТелефона).HasMaxLength(255);
            //
            // entity.Property(e => e.ОбращатьсяК).HasMaxLength(50);
            //
            // entity.Property(e => e.Покупатель).HasColumnName("покупатель");
            //
            // entity.Property(e => e.Поставщик).HasColumnName("поставщик");
            //
            // entity.Property(e => e.ПочтовыйИндекс).HasMaxLength(250);
            //
            // entity.Property(e => e.Уведомление).HasMaxLength(50);
            //
            // entity.Property(e => e.Факс).HasMaxLength(30);

            #endregion
        }
    }
}
