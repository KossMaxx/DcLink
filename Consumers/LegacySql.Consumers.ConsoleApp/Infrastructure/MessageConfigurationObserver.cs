using LegacySql.Consumers.ConsoleApp.Extensions;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.PipeConfigurators;

namespace LegacySql.Consumers.ConsoleApp.Infrastructure
{
    public class MessageConfigurationObserver : ConfigurationObserver, IMessageConfigurationObserver
    {
        public  MessageConfigurationObserver(IConsumePipeConfigurator configurator) : base(configurator)
        {
            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator) where TMessage : class
        {
            var specification = new MassTransitLoggerSpecification<TMessage>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
