# Продукт
Модель Product. Таблица "Товары"
| Model                | SQL                     |
|----------------------|-------------------------|
| Position             | Позиция                 |
| Position             | Марка                   |
| ProductTypeId        | КодТипа                 |
| ProductManager       | ProductManager          |
| Subtype              | подтип                  |
| Weight               | вес                     |
| Volume               | обьем                   |
| Height               | height                  |
| Width                | width                   |
| Depth                | depth                   |
| Price                | PRICE                   |
| Class                | klas                    |
| VendorCode           | artikul                 |
| Monitoring           | RRP                     |
| ManualRrp            | manual_rrp              |
| NotInvolvedInPricing | pricealgoritm_ignore    |
| Guarantee            | war                     |
| GuaranteeIn          | warin                   |
| Manufacture          | manufacture             |
| NomenclatureBarcode  | EAN                     |
| NameForPrinting      | beznal                  |
| Unit                 | изм                     |
| Vat                  | VAT                     |
| IsImported           | о_импрот                |
| NomenclatureCode     | KodZED                  |
| IsProductIssued      | contentOK               |
| IsDistribution       | is_distribution         |
| ScanMonitoring       | scan_monitoring         |
| ScanHotline          | scan_hotline            |
| Game                 | game                    |
| Markdown             | уценка                  |
| CurrencyId           | ВалютаТовара            |
| ProductCountryId     | countryOfOrigin_ID      |
| BrandCountryId       | countryOfRegistration_ID|

## Картинки
Product.Pictures 
Модель ProductPicture. Таблица "PicturesUrl"
| Model          | SQL            |
|----------------|----------------|
| ProductId      | tovID          |
| Url            | url            |
| Date           | ddd            |

## Описания 
Таблица "TBL_description"
| Model          | SQL            |
|----------------|----------------|
| ProductId      |tovID           |
| Language       |language        |
| Description    |description     |
| Date           |ddd             |

## Параметры 
Таблица "KategoryTovars"
| Model          | SQL            |
|----------------|----------------|
| ProductId      |kodtovara       |
| CategoryId     |katID           |
| Description    |CategoryParamId |


