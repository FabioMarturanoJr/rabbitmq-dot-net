using MassTransit;
using Rabbit.Api.Domain;

namespace Rabbit.Api.Consumers;

public class MessageConsumer(ILogger<MessageConsumer> logger) : IConsumer<Message>
{
    private readonly ILogger<MessageConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<Message> context)
    {
        var message = context.Message;

        var rand = new Random();
        var randomNumber = rand.Next(10).ToString();
        if (message.Texto.Contains(randomNumber))
        {
            throw new Exception($" \"{message.Texto}\" contem: {randomNumber}");
        }

        _logger.LogWarning($"MessageConsumer - Aguardando Mensagem: \"{message.Texto}\"");
        await Task.Delay(10000);
        _logger.LogWarning($"MessageConsumer - Mensagem: {message.Texto}, Origem: {message.Origem:g}, HoraMensagem: {message.DataEnvio}, " + $"HoraConsumo: {DateTime.Now}");
    }
}
