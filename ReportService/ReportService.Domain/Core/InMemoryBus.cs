using MediatR;

namespace ShareLib;

public sealed class InMemoryBus : IMediatorHandler
{
    private readonly IMediator _mediator;

    public InMemoryBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return Publish(command);
    }

    public Task RaiseEvent<T>(T @event) where T : Event
    {
        return Publish(@event);
    }

    private Task Publish<T>(T message) where T : Message
    {
        return _mediator.Publish(message);
    }
}
