using Basket.API.GrpcServices;
using Basket.API.Mapper;
using Basket.API.Repositories;
using Common.Logging;
using Discount.Grpc.Protos;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.UseEnrichedSerilog();

builder.Services.AddControllers();


var redisConnectionString = builder.Configuration["CacheSettings:ConnectionString"];
var rabbitMQHostAddress = builder.Configuration["EventBusSettings:HostAddress"];

builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, "Redis Health", HealthStatus.Degraded);

builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]);
});

builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddAutoMapper(typeof(BasketProfile));

builder.Services.AddMassTransit(mt => mt.UsingRabbitMq((_, mq) => mq.Host(rabbitMQHostAddress)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.Run();
