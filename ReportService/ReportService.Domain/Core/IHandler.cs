namespace ShareLib;

public interface IHandler<in T> where T : Message
{
    void Handle(T message);
}