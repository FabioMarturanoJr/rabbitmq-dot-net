using Quartz;
using Rabbit.Api.Domain;

namespace Rabbit.Api.Service.Job;

public class SendMessageJob(ILogger<SendMessageJob> logger, IBusService busService) : IJob
{
    private readonly ILogger<SendMessageJob> _logger = logger;
    private readonly IBusService _busService = busService;

    public async Task Execute(IJobExecutionContext context)
    {
        await _busService.Send([new Message("Mensagem via Job", DateTime.Now, Origem.Job)]);
        _logger.LogInformation($"Mensagem Envia pelo Job {DateTime.Now}!!!");
    }
}
