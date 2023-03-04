using System.Threading.Tasks;
using LegacySql.Domain.Extensions;
using MassTransit;
using Serilog;

namespace LegacySql.Consumers.ConsoleApp
{
    public abstract class BaseConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var type = GetType();
            var typeName = type.Name;
            var entityName = type.GetEntityName();
            var tryCount = context.GetRetryAttempt();
            if (tryCount > 0)
            {
                Log.Logger
                    .Information("LegacySqlConsumers | " + entityName + " | Consumer: {Type} | Retry № {TryCount}", typeName, tryCount);
            }
            Log.Logger
                .Information("LegacySqlConsumers | " + entityName + " | Consumer: {Type}", typeName);
            await ConsumeMessage(context);
        }

        public abstract Task ConsumeMessage(ConsumeContext<TMessage> context);
    }
}
