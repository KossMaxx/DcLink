using System;
using LegacySql.Consumers.ConsoleApp.Infrastructure;
using MassTransit;

namespace LegacySql.Consumers.ConsoleApp.Extensions
{
    public static class MassTransitExtensions
    {
        public static void UseCustomLogger(this IConsumePipeConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new MessageConfigurationObserver(configurator);
        }
    }
}
