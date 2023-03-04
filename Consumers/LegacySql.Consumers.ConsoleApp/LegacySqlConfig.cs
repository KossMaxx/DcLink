﻿ using System;
  using LegacySql.Consumers.Commands;

  namespace LegacySql.Consumers.ConsoleApp
{
    public class LegacySqlConfig
    {
        public ConnectionStringsConfig ConnectionStrings  { get; set; }
        public RabbitMqConfig RabbitMq  { get; set; }
        
        /// <summary>
        /// Строки подключения к базам данных
        /// </summary>
        public class ConnectionStringsConfig
        {
            /// <summary>
            /// Строка подключения к базе данных устаревшего DCLink LegacyDb
            /// </summary>
            public string LegacyDbContext { get; set; }
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
            public string HostAddress  { get; set; }
            /// <summary>
            /// Имя пользователя
            /// </summary>
            public string Username  { get; set; }
            /// <summary>
            /// Пароль пользователя
            /// </summary>
            public string Password  { get; set; }

            private ushort DefaultRabbitPort => 5672;

            private Uri RabbitMqConnectionString => new Uri(HostAddress);

            public string Host => $"{RabbitMqConnectionString.Host}";

            public ushort Port =>
                (ushort)(RabbitMqConnectionString.Port == -1
                    ? DefaultRabbitPort
                    : RabbitMqConnectionString.Port);
        }
    }
}