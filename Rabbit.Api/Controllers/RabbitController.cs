using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Rabbit.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitController : ControllerBase
    {
        private readonly ILogger<RabbitController> _logger;
        private readonly IConnectionFactory _factory;

        private readonly string queue = "rabbit-queue";
        private readonly string queueError = "rabbit-queue-error";

        public RabbitController(ILogger<RabbitController> logger)
        {
            _logger = logger;
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "user", Password = "password" };
        }

        [HttpPost("[action]")]
        public ActionResult<string> EnviarMensagem(int TotalMessagem)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue, false, false, false, null);

            for (int i = 1; i <= TotalMessagem; i++)
            {
                var message = new Message
                {
                    Texto = i.ToString(),
                    DataEnvio = DateTime.Now,
                };

                channel.BasicPublish(string.Empty, "rabbit-queue", null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
                _logger.LogInformation($"Mensagem: \"{i}\" Sucesso");
            }
            return Ok();
        }


        [HttpGet("[action]")]
        public ActionResult<List<Message>> UpConsumers()
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();
            var channelError = connection.CreateModel();


            channel.QueueDeclare(queue, false, false, false, null);
            channelError.QueueDeclare(queueError, false, false, false, null);

            channel.BasicQos(0, 5, false);

            BuildConsumer(channel, channelError, "Consumer 1", _logger, queue, queueError);
            BuildConsumer(channel, channelError, "Consumer 2", _logger, queue, queueError);
            BuildConsumer(channel, channelError, "Consumer 3", _logger, queue, queueError);
            BuildConsumer(channel, channelError, "Consumer 4", _logger, queue, queueError);

            return Ok();
        }

        public static void BuildConsumer(IModel channel, IModel channelError, string WorkerName, ILogger logger, string queue, string queueError)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Task.Delay(1000).Wait();
                var body = ea.Body.ToArray();
                try
                {
                    var message = JsonSerializer.Deserialize<Message>(Encoding.UTF8.GetString(body));
                    if (message != null)
                    {
                        // Error Aleatorio
                        if (message.Texto.Contains("10"))
                        {
                            throw new Exception();
                        }
                        logger.LogWarning($"Mensagem: {message.Texto}, HoraMensagem: {message.DataEnvio}, {WorkerName}, HoraConsumo: {DateTime.Now}");
                    }
                }
                catch (Exception ex)
                {
                    var error = new MessageErro
                    {
                        Message = Encoding.UTF8.GetString(body),
                        ErrorMessage = ex.Message
                    };

                    channelError.BasicPublish(string.Empty, queueError, null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(error)));
                    logger.LogError($"Message com Erro: {Encoding.UTF8.GetString(body)}");
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(queue, false, consumer);
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
