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
        public async Task<ActionResult<string>> EnviarMensagem(int TotalMessagem)
        {
            var messages = new List<Message>();
            for (int i = 0; i < TotalMessagem; i++)
            {
                messages.Add(new Message(i.ToString(), DateTime.Now));
            }
            await _BusService.Send(messages);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> EnviarOutraMensagem(int TotalMessagem)
        {
            var messages = new List<Message2>();
            for (int i = 0; i < TotalMessagem; i++)
            {
                messages.Add(new Message2(i.ToString(), DateTime.Now));
            }
            await _BusService.SendOtherMessage(messages);
            return Ok();
        }
    }
}
