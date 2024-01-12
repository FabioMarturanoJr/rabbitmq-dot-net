using Rabbit.Api.Domain;

namespace Rabbit.Api.Service;

public interface IRabbitService
{
    public Task Send(List<Message> messages);
}
