using Microsoft.AspNetCore.Mvc;
using Quartz;
using Rabbit.Api.Service.Job;

namespace Rabbit.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JobController(ISchedulerFactory factory) : ControllerBase
    {
        private readonly ISchedulerFactory _factory = factory;

        [HttpGet("[action]")]
        public async Task<ActionResult> Run()
        {
            var scheduler = await _factory.GetScheduler();
            await scheduler.TriggerJob(new JobKey(nameof(SendMessageJob)));
            return Ok();
        }
    }
}
