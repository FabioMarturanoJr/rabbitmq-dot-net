using MassTransit;
using Rabbit.Api.Configs;
using Rabbit.Api.Consumers;
using Rabbit.Api.Domain;
using Rabbit.Api.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Config
var massTransitConfigs = builder.Configuration.GetSection(nameof(MassTransitConfigs)).Get<MassTransitConfigs>();

// Service
builder.Services.AddScoped<IBusService, BusService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host($"rabbitmq://{massTransitConfigs?.host}", h =>
        {
            h.Username(massTransitConfigs?.User);
            h.Password(massTransitConfigs?.Pwd);
        });
        cfg.ReceiveEndpoint(Queues.Defaut, ep =>
        {
            ep.ConcurrentMessageLimit = 2;
            ep.PrefetchCount = 10;
            ep.UseMessageRetry(r => r.Interval(2, 10000));
            ep.ConfigureConsumer<MessageConsumer>(context);
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
