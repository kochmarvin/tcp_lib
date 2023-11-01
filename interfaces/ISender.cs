using System.Net;

namespace netlib.interfaces;

internal interface ISender
{
    void SendMessage(byte[] data, IPEndPoint receiver, CancellationToken cancellationToken = default);
}