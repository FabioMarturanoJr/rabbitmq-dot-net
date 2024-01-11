using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Rabbit.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitController(ILogger<RabbitController> logger) : ControllerBase
    {
        private readonly ILogger<RabbitController> _logger = logger;
        private readonly string queue = "rabbit-queue";
        private readonly string queueError = "rabbit-queue-error";
        private readonly List<Message> mensagens = new();

        [HttpPost("[action]")]
        public ActionResult<string> EnviarMensagem(int TotalMessagem)
        {

            var factory = new ConnectionFactory { HostName = "localhost", UserName = "user", Password = "password" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            for (int i = 1; i <= TotalMessagem; i++)
            {
                var message = new Message
                {
                    Texto = i.ToString(),
                    DataEnvio = DateTime.Now,
                };

                channel.BasicPublish(exchange: string.Empty, routingKey: "rabbit-queue", basicProperties: null, body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
                _logger.LogInformation($"Mensagem: \"{i}\" Sucesso");
            }
            return Ok();
        }


        [HttpGet("[action]")]
        public ActionResult<List<Message>> LerMensagensAutoAck()
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "user", Password = "password" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            using var channelError = connection.CreateModel();


            channel.QueueDeclare(queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channelError.QueueDeclare(queueError, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // TODO
            // channel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                try
                {
                    var message = JsonSerializer.Deserialize<Message>(Encoding.UTF8.GetString(body));
                    if (mensagens != null)
                    {
                        // Error Aleatorios
                        if (message.Texto.Contains("10"))
                        {
                            throw new Exception();
                        }
                        _logger.LogInformation($"Mensagem: {message.Texto}, Hora: {message.DataEnvio}");
                    }
                }
                catch (Exception ex)
                {
                    var error = new MessageErro
                    {
                        Message = Encoding.UTF8.GetString(body),
                        ErrorMessage = ex.Message
                    };

                    channelError.BasicPublish(exchange: string.Empty, routingKey: queueError, basicProperties: null, body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(error)));
                    _logger.LogError($"Message com Erro: {Encoding.UTF8.GetString(body)}");
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
            Task.Delay(1000).Wait();
            return Ok();
        }
    }


    public class Message
    {
        public string Texto { get; set; }
        public DateTime DataEnvio { get; set; }
    }

    public class MessageErro
    {
        public string ErrorMessage { get; set; }
        public string Message {  get; set; }
    }
}
