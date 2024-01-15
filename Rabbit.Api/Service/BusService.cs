using MassTransit;
using Microsoft.Extensions.Options;
using Rabbit.Api.Configs;
using Rabbit.Api.Domain;

namespace Rabbit.Api.Service;

public class BusService(IBus bus, ILogger<BusService> logger, IOptions<MassTransitConfigs> MassTransitConfigs) : IBusService
{
    private readonly IBus _bus = bus;
    private readonly ILogger<BusService> _logger = logger;
    private readonly MassTransitConfigs _MassTransitConfigs = MassTransitConfigs.Value;

    public async Task Send(List<Message> messages)
    {
        var endpoint = await _bus.GetSendEndpoint(new Uri($"{_MassTransitConfigs.host}/{Queues.Defaut}"));
        for (int i = 0; i < messages.Count; i++)
        {
            await endpoint.Send(messages[i]);
            _logger.LogInformation($"Mensagem \"{i + 1}\" Enviada Com Sucesso");
        }
    }
}
