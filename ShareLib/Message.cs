using MediatR;

namespace ShareLib;

public abstract class Message : INotification
{
    public string MessageType { get; protected set; }

    protected Message()
    {
        MessageType = GetType().Name;
    }
}