using Domain.ExternalPorts;
using MediatR;
using System.Text;
using System.Text.Json;

namespace Domain.UseCases;

public class MakeCreditOperationEventHandler : INotificationHandler<MakeCreditOperationEvent>
{
    private readonly IMessageQueueManager _messageQueueManager;

    public MakeCreditOperationEventHandler(IMessageQueueManager messageQueueManager)
    {
            _messageQueueManager = messageQueueManager;
    }

    public Task Handle(MakeCreditOperationEvent notification, CancellationToken cancellationToken)
    {
        var stringfiedMessage = JsonSerializer.Serialize(notification);
        _messageQueueManager.PublishMessage("Operation",Encoding.UTF8.GetBytes(stringfiedMessage));
        return Task.CompletedTask;
    }
}