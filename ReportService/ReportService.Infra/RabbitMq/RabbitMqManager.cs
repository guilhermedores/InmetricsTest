using Domain.Entities;
using Domain.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShareLib;
using System.Text;
using System.Text.Json;

public class RabbitMqManager : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly string _exchange;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _queueName = _channel.QueueDeclare().QueueName;
        _exchange = "Operation";

        _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Fanout);

        _channel.QueueBind(queue: _queueName,
                          exchange: _exchange,
                          routingKey: string.Empty);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            ReceiveOperationMessage(message);
        };
        _channel.BasicConsume(queue: _queueName,
                             autoAck: true,
                             consumer: consumer);

        return Task.CompletedTask;
    }

    public void ReceiveOperationMessage(string message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            Operation? obj = JsonSerializer.Deserialize<Operation>(message);
            if (obj != null)
            {
                ReceiveOperationCommand receive = new ReceiveOperationCommand(obj.OperationCode, obj.OperationType, obj.Value, obj.OperationDate);

                var _handler = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

                _handler.SendCommand(receive);
            }
        }
    }
}
