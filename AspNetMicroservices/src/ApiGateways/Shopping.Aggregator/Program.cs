using Common.Logging;
using HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Pollycies;
using Serilog;
using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.UseEnrichedSerilog();

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:CatalogUrl"]}/swagger/index.html"), "Catalog.API", HealthStatus.Degraded)
    .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:BasketUrl"]}/swagger/index.html"), "Basket.API", HealthStatus.Degraded)
    .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:OrderingUrl"]}/swagger/index.html"), "Ordering.API", HealthStatus.Degraded);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(client =>
        client.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
    .AddDefaultPollycies();


builder.Services.AddHttpClient<IBasketService, BasketService>(client =>
        client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
    .AddDefaultPollycies();

builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
        client.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
    .AddDefaultPollycies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecksDefault();

app.Run();
