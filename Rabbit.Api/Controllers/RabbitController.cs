using Microsoft.AspNetCore.Mvc;
using Rabbit.Api.Domain;
using Rabbit.Api.Service;

namespace Rabbit.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitController(IRabbitService rabbitService) : ControllerBase
    {
        private readonly IRabbitService _rabbitService = rabbitService;

        [HttpPost("[action]")]
        public ActionResult<string> EnviarMensagem(int TotalMessagem)
        {
            var messages = new List<Message>();
            for (int i = 0; i < TotalMessagem; i++)
            {
                messages.Add(new Message {
                    Texto = i.ToString(),
                    DataEnvio = DateTime.Now,
                });
            }
            _rabbitService.Send(messages);
            return Ok();
        }
    }
}
