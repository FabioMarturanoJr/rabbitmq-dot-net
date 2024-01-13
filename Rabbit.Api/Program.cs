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
var rabbitConfigSection = builder.Configuration.GetSection(nameof(RabbitConfigs));
var rabbitConfigs = rabbitConfigSection.Get<RabbitConfigs>();

// Service
builder.Services.AddScoped<IRabbitService, RabbitService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host($"rabbitmq://{rabbitConfigs?.host}", h =>
        {
            h.Username(rabbitConfigs?.User);
            h.Password(rabbitConfigs?.Pwd);
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
