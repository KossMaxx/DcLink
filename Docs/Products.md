# Продукт
Модель Product. Представление "lSQL_v_Товары"
| Model                | SQL                     |
|----------------------|-------------------------|
| Code                 | КодТовара               |
| Brand                | Марка                   |
| WorkName             | Позиция                 |
| ProductTypeId        | КодТипа                 |
| Subtype              | подтип                  |
| ProductCategory      | klas                    |
| Manufacture          | manufacture             |
| VendorCode           | artikul                 |
| NomenclatureBarcode  | EAN                     |
| NameForPrinting      | beznal                  |
| ProductCountryId     | countryOfOrigin_ID      |
| BrandCountryId       | countryOfRegistration_ID|
| Weight               | вес                     |
| Volume               | обьем                   |
| PackageQuantity      | kolpak                  |
| Guarantee            | war                     |
| GuaranteeIn          | warin                   |
| Unit                 | изм                     |
| Vat                  | VAT                     |
| IsImported           | о_импрот                |
| NomenclatureCode     | KodZED                  |
| IsProductIssued      | contentOK               |
| ContentUser          | contentUser             |
| InStock              | нал_ф                   |
| InReserve            | нал_резерв              |
| Pending              | нал_ожид                |
| VideoUrl             | videoURL                |
| IsDistribution       | is_distribution         |
| ScanMonitoring       | scan_monitoring         |
| ScanHotline          | scan_hotline            |
| Game                 | game                    |
| ManualRrp            | manual_rrp              |
| NotInvolvedInPricing | pricealgoritm_ignore    |
| Monitoring           | RRP                     |
| Price                | PRICE                   |
| Markdown             | уценка                  |
| LastChangeDate       | DataLastChange          |
| NonCashProductId     | beznal_tovID            |
| Currency             | ВалютаТовара            |
| ManagerNickName      | ProductManager          |
| Price0               | Цена0                   |
| Price1               | Цена1                   |
| DistributorPrice     | Цена2                   |
| RRPPrice             | Цена3                   |
| SpecialPrice         | Цена4                   |
| MinPrice             | Цена5                   |
| InternetPrice        | ЦенаИ                   |
| FirstCost            | SS                      |
| DataLastPriceChange  | DataLastPriceChange     |
| PriceMinBnuah        | PriceMinBNuah           |
| Height               | height                  |
| Width                | width                   |
| Depth                | depth                   |

## Картинки
Product.Pictures 
Модель ProductPictureEF. Таблица "PicturesUrl"
| Model          | SQL            |
|----------------|----------------|
| Id             | picID          |
| ProductId      | tovID          |
| Url            | url            |
| Date           | ddd            |
| Uuu            | uuu            |
| CobraPic       | cobra_pic      |

## Видео
Product.Video
Модель ProductVideoEF. Таблица "VideoURL"
| Model          | SQL            |
|----------------|----------------|
| Id             | id             |
| ProductId      | item_id        |
| Url            | url            |
| Date           | ddd            |


## Параметры получения
Из представления "lSQL_v_Товары" получаются только товары, у которых поле ProductTypeId не равно 244, 129, 247, 431, 502, 798, 805, 828, 833, 889, 1022, 101, 102, 177 и имя типа продукта не содержит фразу "не использовать". 
После идет проверка: "Если у продукта поле WorkName пустое и он не состоит ни в каком заказе, то этот продукт не публикуется".

Дополнительные запросы генерируются при:
1. получении стран по ProductCountryId и BrandCountryId
## Страна
Country
Модель CountryEF Таблица "TBL_CountryNames"
| Model          | SQL            |
|----------------|----------------|
| Id             | CN_ID          |
| IsoId          | C_ID           |
| Title          | CN_Name        |
| LanguageId     | L_ID           |

## Iso код
Country.Iso
Модель CountryIsoCodeEF Таблица "TBL_Countries"
| Model          | SQL            |
|----------------|----------------|
| Id             | C_ID           |
| Code           | C_ISO2_code    |

2. получении параметров категории товара
## Параметры категории товара
Модель ProductCategoryParameterEF Таблица "KategoryTovars"
| Model          | SQL            |
|----------------|----------------|
| Id             | id             |
| ProductId      | kodtovara      |
| CategoryId     | katID          |
| ParameterId    | param          |

3. получении описания товара на украинском и русском языках

### Дата последнего изменения
Из БД получаются только записи, которые были изменены позже указанной даты. Если значение параметра не задано, то получаются все записи(первичная синхронизация). Анализируется поле LastChangeDate. 