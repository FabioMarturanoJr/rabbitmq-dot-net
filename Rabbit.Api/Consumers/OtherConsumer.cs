using MassTransit;
using Rabbit.Api.Domain;

namespace Rabbit.Api.Consumers;

public class OtherConsumer(ILogger<OtherConsumer> logger) : IConsumer<Message2>
{
    private readonly ILogger<OtherConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<Message2> context)
    {
        var message = context.Message;

        var rand = new Random();
        var randomNumber = rand.Next(10).ToString();
        if (message.Texto.Contains(randomNumber))
        {
            throw new Exception($" \"{message.Texto}\" contem: {randomNumber}");
        }

        _logger.LogWarning($"OtherConsumer - Aguardando Mensagem: \"{message.Texto}\"");
        await Task.Delay(10000);
        _logger.LogWarning($"OtherConsumer - Mensagem: {message.Texto}, Origem: {message.Origem:g}, HoraMensagem: {message.DataEnvio}, " + $"HoraConsumo: {DateTime.Now}");
    }
}
