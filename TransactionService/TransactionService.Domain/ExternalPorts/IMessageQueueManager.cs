namespace Domain.ExternalPorts;

public interface IMessageQueueManager
{
    public void PublishMessage(string exchange, byte[] message);
}
