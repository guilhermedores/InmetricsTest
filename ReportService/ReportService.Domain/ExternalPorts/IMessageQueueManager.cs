namespace Domain.ExternalPorts;

public interface IMessageQueueManager
{
    void SubscribeMessage(string exchange);
}
