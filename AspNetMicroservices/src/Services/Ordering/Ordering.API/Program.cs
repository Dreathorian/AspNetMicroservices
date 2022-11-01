using Common.Logging;
using EventBus.Messages.Common;
using EventBus.Messages.Events;
using HealthChecks;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.API.Mapping;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.UseEnrichedSerilog();

builder.Services.AddHealthChecks().AddDbContextCheck<OrderContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMassTransit(mtConfig =>
{
    mtConfig.AddConsumer<BasketCheckoutConsumer>();

    mtConfig.UsingRabbitMq((context, mqConfig) =>
    {
        mqConfig.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        mqConfig.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(context);
        });
    });
});

builder.Services.AddAutoMapper(typeof(OrderingProfile));
builder.Services.AddScoped<BasketCheckoutConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateDatabase<OrderContext>((context, provider) =>
    OrderContextSeed.SeedAsync(context, provider.GetRequiredService<ILogger<OrderContextSeed>>()).Wait());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecksDefault();

app.Run();
