using Rabbit.Api.Domain;

namespace Rabbit.Api.Service;

public interface IBusService
{
    public Task Send(List<Message> messages);
    public Task SendOtherMessage(List<Message2> messages);
}
