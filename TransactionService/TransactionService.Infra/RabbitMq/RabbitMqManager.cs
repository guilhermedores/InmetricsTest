using Domain.ExternalPorts;
using RabbitMQ.Client;


public class RabbitMqManager : IMessageQueueManager
{
    public void PublishMessage(string exchange, byte[] message)
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);
        
        channel.BasicPublish(exchange: exchange,
                             routingKey: string.Empty,
                             basicProperties: null,
                             body: message);
    }
}
