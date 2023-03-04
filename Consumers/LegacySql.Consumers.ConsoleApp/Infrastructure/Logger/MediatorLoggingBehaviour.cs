using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Extensions;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace LegacySql.Consumers.ConsoleApp.Infrastructure.Logger
{
    public class MediatorLoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            Type myType = request.GetType();
            var entityName = myType.GetEntityName();
            var props = new List<PropertyInfo>(myType.GetProperties());
            var properties = new Dictionary<string, object>();
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(request, null);
                properties.Add(prop.Name, JsonConvert.SerializeObject(propValue));
            }

            var requestType = typeof(TRequest);
            var infoString = new StringBuilder(" {Type}");
            if (requestType.IsGenericType)
            {
                infoString.Append(" for {InnerType}");
                if (requestType.GetGenericArguments().Length > 0)
                {
                    entityName = requestType.GetGenericArguments()[0].GetEntityName();
                }
            }

            var timer = new Stopwatch();
            timer.Start();
            var response = await next();
            timer.Stop();

            Log.Logger.ForContext("ObjectType", entityName)
                .ForContext("RequestProperties", properties)
                .ForContext("ResponseType", typeof(TResponse).Name)
                .ForContext("ResponseValue", JsonConvert.SerializeObject(response))
                .Information("LegacySqlConsumers | " + entityName + " | Command:" + infoString + " in {Elapsed} ms success",
                    requestType.Name,
                    requestType.IsGenericType && requestType.GetGenericArguments().Length > 0 ? requestType.GetGenericArguments()[0].Name : timer.Elapsed.ToString(),
                    timer.Elapsed);

            return response;
        }
    }
}
