## Развертывание development окружения
1. Установка docker
2. Запуск postgre-sql контейнера
3. Создание баз данных. Применение миграций
4. Развертывание rabbitmq контейнера
6. Резвертывание seq контейнера (опционально). Логи также пишутся в консоль 
7. Клонирование репозитория
8. Заполнение параметров конфигурации

## Параметры конфигурации

Можно задать двумя способами: 

1. Файл appsettings.json. Для не secure параметров, которые одинаковые для всех экземпляров контейнеров  

2. Env variables. Для secure параметров, которые нельзя хранить в коде и которые отличаются для разных контейнеров  

### Правила именования переменных окружения
Для получения вложенных полей в коде используется ":"  
В переменных окружения ему соответствует два нижних подчеркивания "__"

Пример для строки подключения к БД legacy_sql

- "ConnectionStrings:LegacyDbContext" - ключ использования коде

- "ConnectionStrings__LegacyDbContext" - имя переменной окружения

### Development env 
Параметры конфигурации, которые отличаются от стандартных значений из appsettings.json можно переопределить в файле launchSettings.json. 

Зададать переменные окружения можно в секции "environmentVariables" для соответствующего профиля.

Пример launchSettings.json
```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "LegacySql.Api": {
      "commandName": "Project",
      "environmentVariables": 
      {        
        "ASPNETCORE_ENVIRONMENT": "Development",        
        "ConnectionStrings__AppDbContext": "Server=localhost; User Id=postgres; Password=postgres; Database=dc_link_legacy_sql;",
        "ConnectionStrings__LegacyDbContext": "Server=;Database=; User Id=tqm; Password=; Connection Timeout=300",
        "RabbitMq__HostAdress": "rabbitmq://localhost",
        "RabbitMq__Password": "guest",
        "RabbitMq__Username": "guest",
        "LegacySqlFilters__NotFullMappingIdPortion": "15000",
        "LegacySqlFilters__ClientOrderPeriod": "2020.10.01",
        "Seq_Url": "http://localhost:5341"
      },
      "applicationUrl": "http://localhost:5000/"
    }
  }
}
```
## Работа с миграциями БД

Для локального использования миграций Udpate-Database, Add-Migration ...
В Package Manger Console выполнить команду
```powershell
$env:DC_LINK_LEGACY_SQL_APP_CONN_STRING = "Server=localhost; Port=5433; User Id=postgres; Password=postgres; Database=legacy_sql;";
```

## Запуск в Docker
### На машине разработчика
Сборка
``` 
docker build -t legacy-sql .
```
Запуск
```
docker run -it --restart always -p 5001:80 -e ConnectionStrings__AppDbContext="Server=localhost; User Id=postgres; Password=postgres; Database=legacy_sql;"  -e ConnectionStrings__LegacyDbContext="Server=; Database=; User Id=tqm; Password=; Connection Timeout=300" -e RabbitMq__HostAdress="rabbitmq://host.docker.internal" -e RabbitMq__Username=guest -e RabbitMq__Password=guest -e LegacySqlFilters__NotFullMappingIdPortion=15000 -e LegacySqlFilters__SellingPricesTimePeriod=-42 Seq_Url="http://localhost:5341"  --name legacy-sql legacy-sql
```
### В рабочем окружении
Сборка
```
docker build -t legacy-sql --file Dockerfile.LegacySql <git repo url>
docker build -t legacy-sql-consumers --file Dockerfile.Consumers <git repo url>
```
Запуск 
dev env
```
Legasy-Sql
docker run -it --cpus=".25" --restart always -p 5001:80 -e ConnectionStrings__AppDbContext="Server=172.17.0.1; User Id=postgres; Password=; Database=legacy_sql_dev;"  -e ConnectionStrings__LegacyDbContext="Server=; Database=; User Id=tqm; Password=; Connection Timeout=300" -e RabbitMq__HostAdress="rabbitmq://rabbit.dclink.ua" -e RabbitMq__Username=admin -e RabbitMq__Password= -e LegacySqlFilters__NotFullMappingIdPortion=15000 -e LegacySqlFilters__ClientOrderPeriod="2021.01.01" -e Seq_Url="" --name legacy-sql-dev legacy-sql
```
```
legacy-sql-consumers
docker run -it --cpus=".25" --restart always -e ConnectionStrings__AppDbContext="Server=172.17.0.1; User Id=postgres; Password=; Database=legacy_sql_dev;" -e RabbitMq__HostAdress="rabbitmq://rabbit.dclink.ua" -e RabbitMq__Username=admin -e RabbitMq__Password= Seq_Url="" --name legacy-sql-consumers-dev legacy-sql-consumers
```

prod env
```
Legasy-Sql
docker run -it --cpus=".25" --restart always -p 5002:80 -e ConnectionStrings__AppDbContext="Server=172.17.0.1; User Id=postgres; Password=; Database=legacy_sql_production;"  -e ConnectionStrings__LegacyDbContext="Server=; Database=; User Id=LegacySql; Password=; Connection Timeout=300" -e RabbitMq__HostAdress="rabbitmq://" -e RabbitMq__Username=admin -e RabbitMq__Password= -e LegacySqlFilters__NotFullMappingIdPortion=15000 -e LegacySqlFilters__ClientOrderPeriod="2021.01.01" -e Seq_Url="" --name legacy-sql-production legacy-sql
```
```
legacy-sql-consumers
docker run -it --cpus=".25" --restart always -e ConnectionStrings__AppDbContext="Server=172.17.0.1; User Id=postgres; Password=; Database=legacy_sql_production;" -e RabbitMq__HostAdress="rabbitmq://" -e RabbitMq__Username=admin -e RabbitMq__Password= Seq_Url="" --name legacy-sql-consumers-production legacy-sql-consumers
```