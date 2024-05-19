using System;

namespace ShareLib;

public abstract class Event : Message
{
    public DateTime Timestamp { get; private set; }

    protected Event()
    {
        Timestamp = DateTime.Now;
    }
}