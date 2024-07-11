using MassTransit;
using Quartz;
using Rabbit.Api.Configs;
using Rabbit.Api.Consumers;
using Rabbit.Api.Domain;
using Rabbit.Api.Service;
using Rabbit.Api.Service.Job;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Config
var massTransitConfigsSection = builder.Configuration.GetSection(nameof(MassTransitConfigs));
builder.Services.Configure<MassTransitConfigs>(massTransitConfigsSection);

// Service
var massTransitConfigs = massTransitConfigsSection.Get<MassTransitConfigs>();

builder.Services.AddScoped<IBusService, BusService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();
    x.AddConsumer<OtherConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(massTransitConfigs?.host, h =>
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
            ep.ConfigureConsumer<OtherConsumer>(context);
        });
    });
});

// Job
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey(nameof(SendMessageJob));
    q.AddJob<SendMessageJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity($"{nameof(SendMessageJob)}-trigger")
        .WithCronSchedule(builder.Configuration["JobConfigs:CronExpression"]!));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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
