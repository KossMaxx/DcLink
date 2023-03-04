﻿ using System;

  namespace LegacySql.Api
{
    public class LegacySqlConfig
    {
        public ConnectionStringsConfig ConnectionStrings  { get; set; }
        public RabbitMqConfig RabbitMq  { get; set; }
        public LegacySqlFiltersConfig LegacySqlFilters { get; set; }
        
        /// <summary>
        /// Строки подключения к базам данных
        /// </summary>
        public class ConnectionStringsConfig
        {
            /// <summary>
            /// Строка подключения к базе данных устаревшего DCLink LegacyDb
            /// </summary>
            public string LegacyDbContext  { get; set; }
            /// <summary>
            /// Строка подключения к базе данных маппинга AppDb
            /// </summary>
            public string AppDbContext  { get; set; }
        }

        /// <summary>
        /// Строки подключения для RabbitMq
        /// </summary>
        public class RabbitMqConfig
        {
            /// <summary>
            /// Адрес хоста RabbitMq
            /// </summary>
            public string HostAdress  { get; set; }
            /// <summary>
            /// Имя пользователя
            /// </summary>
            public string Username  { get; set; }
            /// <summary>
            /// Пароль пользователя
            /// </summary>
            public string Password  { get; set; }

            private ushort DefaultRabbitPort => 5672;

            private Uri RabbitMqConnectionString => new Uri(HostAdress);

            public string Host => $"{RabbitMqConnectionString.Host}";

            public ushort Port =>
                (ushort)(RabbitMqConnectionString.Port == -1
                    ? DefaultRabbitPort
                    : RabbitMqConnectionString.Port);
        }

        /// <summary>
        /// Переменные среды для выборки данных
        /// </summary>
        public class LegacySqlFiltersConfig
        {
            /// <summary>
            /// Количество элементов в фильтре за один запрос к БД
            /// </summary>
            public string NotFullMappingIdPortion { get; set; }
            /// <summary>
            /// Дата, от которой нужно получать заказы
            /// </summary>
            public string ClientOrderPeriod { get; set; }
        }
    }
}