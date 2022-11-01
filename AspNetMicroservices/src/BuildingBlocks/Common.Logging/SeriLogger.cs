using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging;

public static class SeriLogger
{
    // .net 5
    public static IHostBuilder UseEnrichedSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, configuration) =>
        {
            builder.ConfigureServices(collection => collection.AddTransient<LoggingDelegatingHandler>());

            builder.ConfigureServices(collection => collection.ConfigureAll<HttpClientFactoryOptions>(options =>
                options.HttpMessageHandlerBuilderActions.Add(handlerBuilder =>
                    handlerBuilder.AdditionalHandlers.Add(handlerBuilder.Services.GetRequiredService<LoggingDelegatingHandler>()))));

            configuration.UseElasticEnrich(context.Configuration, context.HostingEnvironment);

        });
    }

    // .net 6
    public static WebApplicationBuilder UseEnrichedSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<LoggingDelegatingHandler>();

        builder.Services.ConfigureAll<HttpClientFactoryOptions>(options =>
            options.HttpMessageHandlerBuilderActions.Add(handlerBuilder =>
                handlerBuilder.AdditionalHandlers.Add(handlerBuilder.Services.GetRequiredService<LoggingDelegatingHandler>())));

        var logger = new LoggerConfiguration();

        logger.UseElasticEnrich(builder.Configuration, builder.Environment);

        builder.Logging.AddSerilog(logger.CreateLogger());
        return builder;
    }

    public static LoggerConfiguration UseElasticEnrich(this LoggerConfiguration logger, IConfiguration configuration, IHostEnvironment environment)
    {
        logger.Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(
                    new Uri(configuration["ElasticConfiguration:Uri"]))
                {
                    IndexFormat =
                        $"applogs-{Assembly.GetEntryAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfShards = 2,
                    NumberOfReplicas = 1,
                })
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .Enrich.WithProperty("Application", environment.ApplicationName)
            .ReadFrom.Configuration(configuration);
        
        return logger;
    }
}
