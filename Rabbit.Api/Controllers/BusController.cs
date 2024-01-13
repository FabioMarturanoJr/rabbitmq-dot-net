using Microsoft.AspNetCore.Mvc;
using Rabbit.Api.Domain;
using Rabbit.Api.Service;

namespace Rabbit.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusController(IBusService BusService) : ControllerBase
    {
        private readonly IBusService _BusService = BusService;

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
            _BusService.Send(messages);
            return Ok();
        }
    }
}
