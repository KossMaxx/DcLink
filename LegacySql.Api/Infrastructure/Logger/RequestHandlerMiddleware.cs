using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace LegacySql.Api.Infrastructure.Logger
{
    public class RequestHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context, LoggerService loggerService)
        {
            loggerService.InitHeader(context);
            await loggerService.LogRequest(context, _recyclableMemoryStreamManager);
            await loggerService.LogResponse(context, _recyclableMemoryStreamManager, _next);
        }
    }
}