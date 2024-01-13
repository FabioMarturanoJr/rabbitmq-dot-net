using MassTransit;
using Microsoft.Extensions.Options;
using Rabbit.Api.Configs;
using Rabbit.Api.Domain;

namespace Rabbit.Api.Service;

public class RabbitService(IBus bus, ILogger<RabbitService> logger, IOptions<RabbitConfigs> rabbitConfigs) : IRabbitService
{
    private readonly IBus _bus = bus;
    private readonly ILogger<RabbitService> _logger = logger;
    private readonly RabbitConfigs _rabbitConfigs = rabbitConfigs.Value;

    public async Task Send(List<Message> messages)
    {
        var endpoint = await _bus.GetSendEndpoint(new Uri($"rabbitmq://{_rabbitConfigs.host}/{Queues.Defaut}"));
        for (int i = 0; i < messages.Count; i++)
        {
            await endpoint.Send(messages[i]);
            _logger.LogInformation($"Mensagem \"{i + 1}\" Enviada Com Sucesso");
        }
    }
}
