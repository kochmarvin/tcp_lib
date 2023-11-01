using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using netlib.interfaces;
using netlib.records;

namespace netlib.tcp;

public class TCPSender(IOptions<TCPSenderOptions> options, ILogger<TCPSender> logger) : ISender
{
    public async void SendMessage(byte[] message, IPEndPoint receiver, CancellationToken cancellationToken = default)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (receiver is null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        try
        {
            TcpClient client = new();
            await client.ConnectAsync(receiver.Address, receiver.Port, cancellationToken);
            using var stream = client.GetStream();
            var buffer = message.AsMemory();
            await stream.WriteAsync(buffer, cancellationToken);
        }
        catch (SocketException)
        {
            logger.LogError("Cannot connect to {address}/{port}", receiver.Address.ToString(), receiver.Port.ToString());
        }
    }
}