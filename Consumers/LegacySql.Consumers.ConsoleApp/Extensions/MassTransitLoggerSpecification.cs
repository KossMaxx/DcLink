using System.Collections.Generic;
using GreenPipes;
using LegacySql.Consumers.ConsoleApp.Infrastructure.Logger;
using MassTransit;

namespace LegacySql.Consumers.ConsoleApp.Extensions
{
    public class MassTransitLoggerSpecification<T> : IPipeSpecification<ConsumeContext<T>> where T : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new MassTransitLogger<T>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}
