# Партнер
Модель Client. Таблица "Клиенты"
| Model                | SQL                     |
|----------------------|-------------------------|
| SupplierCode         | КодПоставщика           |
| Title                | Название                |
| IsSupplier           | Поставщик               |
| IsCustomer           | Покупатель              |
| IsCompetitor         | Konkurent               |
| Email                | email                   |
| MasterId             | masterID                |
| BalanceCurrencyId    | ВалютаБаланса           |
| Department           | department              |
| CreditDays           | kredit                  |
| PriceValidDays       | PriceValidDays          |
| MainManagerId        | manager1                |
| ResponsibleManagerId | manager2                |
| MarketSegmentId      | segmentation            |
| Credit               | кредит                  |
| SurchargePercents    | penyaV                  |
| BonusPercents        | bonusV                  |
| DelayOk              | delayOk                 |
| DeliveryTel          | dostavkaTel             |
| City                 | Город                   |
| Website              | website                 |
| ContactPerson        | ОбращатьсяК             |
| ContactPersonPhone   | НомерТелефона           |
| Address              | Адрес                   |
| MobilePhone          | cell_ID                 |
| DefaultPriceColumn   | колонка                 |

## Адреса доставки
Partner. DeliveryAddresses

Модель ErpClientDeliveryAddressDto. Таблица "TBL_Clients_Shipping_Addr"

`TBL_Clients_Shipping_Addr.client_ID = Клиенты.КодПоставщика`

| Model          | SQL              |
|----------------|------------------|
| Address        | dostavkaAdr      |
| ContactPerson  | dostavkaFIO      |
| Phone          | dostavkaTel      |
| WaybillAddress | WayBIll_addr     |
| Type           | addr_type        |
| Получается из маппинга или созданного партнера       | client_ID        | 

## Приоритеты резервирования
Partner.WarehousePriorities

Модель ErpClientWarehousePriorityDto. Таблица "TBL_Client_Sklad_Reserv_Priority"

`TBL_Client_Sklad_Reserv_Priority.client_id = Клиенты.КодПоставщика`

| Model       | SQL              |
|-------------|------------------|
| Получается из маппинга или созданного партнера    | client_id        |
| WarehouseId | sklad_id         |
| Priority    | order_index      |

## Доступность складов
Partner.WarehouseAccesses

Модель ErpClientWarehouseAccessDto. Таблица "webSkladDopusk"

`webSkladDopusk klientID = Клиенты.КодПоставщика`

| Model       | SQL      |
|-------------|----------|
| Получается из маппинга или созданного партнера   | klientID |
| WarehouseId | sklad    |
| HasAccess   | price    |


# Фирмы
Модель ErpFirmDto. Таблица "Firms"
| Model                | SQL                     |
|----------------------|-------------------------|
| Получается из SQL по ОКПО и klientID| Код      |
| Title                | Название                |
| TaxCode              | ОКПО                    |
| LegalAddress         | Адрес                   |
| Phone                | Телефон                 |
| Account              | Рс                      |
| BankName             | Банк                    |
| BankCode             | МФО                     |
| Получается из маппинга партнера| klientID      |
| Address              | AddressF                |
