using Microsoft.AspNetCore.Mvc;

namespace Rabbit.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitController(ILogger<RabbitController> logger) : ControllerBase
    {
        private readonly ILogger<RabbitController> _logger = logger;

        [HttpGet("[action]")]
        public ActionResult<string> EnviarMensagem(string messagem)
        {
            var result = $"Mensagem: \"{messagem}\" enviada com Sucesso";
            _logger.LogInformation(result);
            return Ok(result);
        }
    }
}
