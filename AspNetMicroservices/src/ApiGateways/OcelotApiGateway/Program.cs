using Common.Logging;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.UseEnrichedSerilog();

builder.Services.AddOcelot().AddConsul()
    .AddCacheManager(part => part.WithDictionaryHandle());

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);

var app = builder.Build();

app.UseOcelot().Wait();

app.MapGet("/", () => "Hello World!");

app.Run();
