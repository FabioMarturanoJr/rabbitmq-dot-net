using MassTransit;
using Rabbit.Api.Domain;

namespace Rabbit.Api.Consumers;

public class MessageConsumer(ILogger<MessageConsumer> logger) : IConsumer<Message>
{
    private readonly ILogger<MessageConsumer> _logger = logger;

    public Task Consume(ConsumeContext<Message> context)
    {
        var message = context.Message;

        if (message.Texto.Contains("10"))
        {
            throw new Exception("Contains 10");
        }

        _logger.LogWarning($"Mensagem: {message.Texto}, HoraMensagem: {message.DataEnvio}, " +
            $"HoraConsumo: {DateTime.Now}");

        return Task.CompletedTask;
    }
}
