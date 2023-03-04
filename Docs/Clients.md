# Баланс
Модель Client. Таблица "Клиенты"
| Model                | SQL                     |
|----------------------|-------------------------|
| Id                   | КодПоставщика           |
| Title                | Название                |
| OnlySuperReports     | H                       |
| IsSupplier           | Поставщик               |
| IsCustomer           | Покупатель              |
| IsCompetitor         | Konkurent               |
| Email                | email                   |
| ChangedAt            | modified_at             |
| MasterId             | masterID                |
| BalanceCurrencyId    | ВалютаБаланса           |
| Department           | department              |
| IsTechnicalAccount   | price_log               |
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
| SegmentAccessories   | segment_accessories     |
| SegmentActiveNet     | segment_active_net      |
| SegmentAv            | segment_AV              |
| SegmentComponentsPc  | segment_componentsPC    |
| SegmentExpendables   | segment_expendables     |
| SegmentKbt           | segment_KBT             |
| SegmentMbt           | segment_MBT             |
| SegmentMobile        | segment_mobile          |
| SegmentNotebooks     | segment_notebooks       |
| SegmentPassiveNet    | segment_passive_net     |
| SegmentPeriphery     | segment_periphery       |
| SegmentPrint         | segment_print           |
| SegmentReadyPc       | segment_readyPC         |
| Consig               | consig                  |
| IsPcAssembler        | is_PC_assembler         |
| SegmentNetSpecifility| segment_net_specifility |
| Website              | website                 |
| ContactPerson        | ОбращатьсяК             |
| ContactPersonPhone   | НомерТелефона           |
| Address              | Адрес                   |
| MobilePhone          | cell_ID                 |
| DefaultPriceColumn   | колонка                 |

## Подчиненные балансы
Client.Nested 

Модель Client. Таблица "Клиенты"

`Клиенты.masterID = Клиенты.КодПоставщика`

## Контрагенты
Client.Firms

Модель Firm. Таблица "Firms"

`Firms.klientID = Клиенты.КодПоставщика`
| Model          | SQL            |
|----------------|----------------|
| Id             | Код            |
| TaxCode        | Окпо           |
| Title          | Название       |
| LegalAddress   | Адрес          |
| Address        | AddressF       |
| Phone          | Телефон        |
| Account        | Рс             |
| BankCode       | Мфо            |
| BankName       | Банк           |
| ClientId       | klientID       |
| LastChangeDate | DataLastChange |

## Адреса доставки
Client.DeliveryAddresses

Модель ClientDeliveryAddress. Таблица "TBL_Clients_Shipping_Addr"

`TBL_Clients_Shipping_Addr.client_ID = Клиенты.КодПоставщика`

| Model          | SQL              |
|----------------|------------------|
| Id             | shipping_addr_ID |
| Address        | dostavkaAdr      |
| ContactPerson  | dostavkaFIO      |
| Phone          | dostavkaTel      |
| WaybillAddress | WayBIll_addr     |
| Type           | addr_type        |
| ClientId       | client_ID        | 

## Приоритеты резервирования
Client.WarehousePriorities

Модель ClientWarehousePriority. Таблица "TBL_Client_Sklad_Reserv_Priority"

`TBL_Client_Sklad_Reserv_Priority.client_id = Клиенты.КодПоставщика`

| Model       | SQL              |
|-------------|------------------|
| Id          | id               |
| ClientId    | client_id        |
| WarehouseId | sklad_id         |
| Priority    | order_index      |

## Доступность складов
Client.WarehouseAccesses

Модель ClientWarehouseAccess. Таблица "webSkladDopusk"

`webSkladDopusk klientID = Клиенты.КодПоставщика`

| Model       | SQL      |
|-------------|----------|
| Id          | id       |
| ClientId    | klientID |
| WarehouseId | sklad    |
| HasAccess   | price    |

## Параметры получения
Из таблицы "Клиенты" получаются только мастер балансы `Клиенты.КодПоставщика = Клиенты.masterID`. Результат содержит все подчиненные балансы, а другие вложенные наборы данных (Контрагенты, Приоритеты резервирования, Доступность складов). 

Дополнительные запросы генерируются только при формировании сообщения по подчиненным балансам. Получаются их вложенные наборы данных (Контрагенты, Приоритеты резервирования, Доступность складов)

### Дата последнего изменения
Из БД получаются только записи, которые были изменены позже указанной даты. Если значение параметра не задано, то получаются все записи(первичная синхронизация). Анализируются несколько дат `Клиенты.modified_at` и `Firms.DataLastChange`. Если дата изменения подчиненного баланса больше указанные даты, то мастер баланс должен попасть в выборку. 

### Список идентификаторов мастер-балансов
Позволяет добавить в выборку списко мастер балансов по списку идентификаторов. Позволяет повторно получить мастер балансы, которые не удалось обработать в момент прошлого запуска. 