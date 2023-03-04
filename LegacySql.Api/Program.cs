using LegacySql.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.WithProperty("Service", "LegacySql")
                .Enrich.FromLogContext()
                .WriteTo.Console(/*new RenderedCompactJsonFormatter()*/)
                .WriteTo.File(path: $"{AppDomain.CurrentDomain.BaseDirectory}/logs/seq_logs_.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} ::::: Properties: {Properties:j}{NewLine}{Exception}",
                    shared: true);
var seqUrl = Environment.GetEnvironmentVariable("Seq_Url");
if (!string.IsNullOrEmpty(seqUrl))
{
    loggerConfiguration = loggerConfiguration.WriteTo.Seq(seqUrl, eventBodyLimitBytes: 500000);
}

Log.Logger = loggerConfiguration.CreateLogger();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Legasy Sql API"
    });
    options.CustomSchemaIds(x => x.FullName);
});

var startup = new Startup(builder.Environment, builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, app.Lifetime);

app.Run();
