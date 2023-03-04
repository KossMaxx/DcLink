using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Serilog;

namespace LegacySql.Api.Infrastructure.Logger
{
    public class LoggerService
    {
        private string _controllerName;
        private string _header;

        public void InitHeader(HttpContext context)
        {
            _controllerName = GetControllerName(context);
            _header = $"LegacySql | {_controllerName} | ";
        }
        public async Task LogRequest(HttpContext context, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            context.Request.EnableBuffering();
            await using var requestStream = recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            Log.Logger.ForContext("ObjectType", _controllerName)
                .ForContext("Schema", context.Request.Scheme)
                .ForContext("Host", context.Request.Host)
                .ForContext("Path", context.Request.Path)
                .ForContext("QueryString", context.Request.QueryString)
                .ForContext("Request Body", await ReadStreamInChunks(requestStream))
                .Information(_header + "Request {Method} {Url}",
                    context.Request.Method,
                    context.Request.Path);
            context.Request.Body.Position = 0;
        }
        private async Task<string> ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = await reader.ReadBlockAsync(readChunk,
                    0,
                    readChunkBufferLength);
                await textWriter.WriteAsync(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }

        public async Task LogResponse(HttpContext context, RecyclableMemoryStreamManager recyclableMemoryStreamManager, RequestDelegate next)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            var timer = new Stopwatch();
            timer.Start();
            try
            {
                await next(context);
                timer.Stop();

                var response = context.Response;
                response.Body.Seek(0, SeekOrigin.Begin);
                var text = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
                if (response.StatusCode == 200)
                {
                    Log.Logger.ForContext("ObjectType", _controllerName)
                        .ForContext("Schema", context.Request.Scheme)
                        .ForContext("Host", context.Request.Host)
                        .ForContext("Path", context.Request.Path)
                        .ForContext("QueryString", context.Request.QueryString)
                        .ForContext("Response Body", text)
                        .ForContext("StatusCode", response.StatusCode)
                        .Information(_header + "Response {Method} {Url} in {Elapsed} ms with code {StatusCode}",
                            context.Request.Method,
                            context.Request.Path,
                            timer.Elapsed,
                            response.StatusCode);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                else
                {
                    Log.Logger.ForContext("ObjectType", _controllerName)
                        .ForContext("Error", text)
                        .Error(_header + "Error HTTP {Method} {Url} in {Elapsed} ms with code {StatusCode}",
                            context.Request.Method,
                            context.Request.Path,
                            timer.Elapsed,
                            response.StatusCode);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception e)
            {
                timer.Stop();
                Log.Logger.ForContext("ObjectType", _controllerName)
                    .ForContext("Error", GetExceptionMessage(e))
                    .ForContext("StackTrace", e.StackTrace)
                    .Error(_header + "Error HTTP {Method} {Url} in {Elapsed} ms with code {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        timer.Elapsed,
                        (int)HttpStatusCode.InternalServerError);
            }
        }

        private string GetControllerName(HttpContext context)
        {
            var controllerName = "";
            var routeData = context.Request.Path.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (routeData.Length > 1)
            {
                controllerName = routeData[1];
            }
            return controllerName;
        }

        private string GetExceptionMessage(Exception e)
        {
            if (e.InnerException != null)
            {
                return GetExceptionMessage(e.InnerException);
            }

            return $"Message: {e.Message}\nStackTrace: {e.StackTrace}";
        }
    }
}
