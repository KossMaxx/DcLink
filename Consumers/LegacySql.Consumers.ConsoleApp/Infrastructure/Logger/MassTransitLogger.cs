using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using GreenPipes;
using LegacySql.Domain.Extensions;
using MassTransit;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;

namespace LegacySql.Consumers.ConsoleApp.Infrastructure.Logger
{
    public class MassTransitLogger<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private string _entityName;
        private string _header;
        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            _entityName = context.Message.GetType().GetEntityName();
            _header = "LegacySqlConsumers | " + _entityName + " |";
            LogContext.PushProperty("TraceId", Guid.NewGuid());
            LogRequest(context);
            var timer = new Stopwatch();
            timer.Start();
            
            

            try
            {
                await next.Send(context);
                timer.Stop();

                if (context.IsResponseAccepted<T>())
                {
                    Log.Logger.ForContext("ObjectType", _entityName)
                        .Information(_header + " Message processed: {Type} success in {Elapsed} ms",
                            context.Message.GetType().Name,
                            timer.Elapsed);
                }
            }
            catch (Exception e)
            {
                timer.Stop();
                Log.Logger.ForContext("ObjectType", _entityName)
                    .ForContext("ErrorMessage", e.Message)
                    .Error(_header + " Error message {Type} in {Elapsed} ms",
                        context.Message.GetType().Name,
                        timer.Elapsed);
            }
        }

        public void Probe(ProbeContext context)
        {
            //var scope = context.CreateFilterScope("messageFilter");
        }

        private void LogRequest(ConsumeContext<T> context)
        {
            Type messageType = context.Message.GetType();
            var props = new List<PropertyInfo>(messageType.GetProperties());
            var properties = new Dictionary<string, object>();
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(context.Message, null);
                properties.Add(prop.Name, JsonConvert.SerializeObject(propValue));
            }

            Log.Logger.ForContext("ObjectType", _entityName)
                .ForContext("MessageProperties", properties)
                .Information(_header + " Message get: {Type}", messageType.Name);
        }
    }
}
