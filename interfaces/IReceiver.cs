using System.Net;
using netlib.eventargs;

namespace netlib.interfaces;

internal interface IReceiver
{
    event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

    void Start();

    void Stop();

    public IPAddress IPAddress
    {
        get;
        set;
    }
}