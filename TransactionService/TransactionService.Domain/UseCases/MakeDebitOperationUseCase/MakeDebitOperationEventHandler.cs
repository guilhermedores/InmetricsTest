using Domain.ExternalPorts;
using MediatR;
using System.Text.Json;
using System.Text;

namespace Domain.UseCases;

public class MakeDebitOperationEventHandler : INotificationHandler<MakeDebitOperationEvent>
{
    private readonly IMessageQueueManager _messageQueueManager;

    public MakeDebitOperationEventHandler(IMessageQueueManager messageQueueManager)
    {
        _messageQueueManager = messageQueueManager;
    }

    public Task Handle(MakeDebitOperationEvent notification, CancellationToken cancellationToken)
    {
        var stringfiedMessage = JsonSerializer.Serialize(notification);
        _messageQueueManager.PublishMessage("Operation", Encoding.UTF8.GetBytes(stringfiedMessage));
        return Task.CompletedTask;
    }
}