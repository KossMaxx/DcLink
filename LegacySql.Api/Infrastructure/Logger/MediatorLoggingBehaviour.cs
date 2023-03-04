using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Extensions;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace LegacySql.Api.Infrastructure.Logger
{
    public class MediatorLoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            Type myType = request.GetType();
            var props = new List<PropertyInfo>(myType.GetProperties());
            var properties = new Dictionary<string, object>();
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(request, null);
                properties.Add(prop.Name, JsonConvert.SerializeObject(propValue));
            }

            var timer = new Stopwatch();
            timer.Start();
            var response = await next();
            timer.Stop();

            var entityName = typeof(TRequest).GetEntityName();
            var header = "LegacySql | " + entityName + " |";

            Log.Logger.ForContext("ObjectType", entityName)
                .ForContext("RequestProperties", properties)
                .ForContext("ResponseType", typeof(TResponse).Name)
                .ForContext("ResponseValue", JsonConvert.SerializeObject(response))
                .Information(header + " Command: {Type} Success in {Elapsed} ms",
                    typeof(TRequest).Name,
                    timer.Elapsed);

            return response;
        }
    }
}
