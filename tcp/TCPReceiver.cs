using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using netlib.eventargs;
using netlib.interfaces;
using netlib.records;

namespace netlib.tcp;

internal class TCPReceiver(IOptions<TCPReceiverOptions> options, ILogger<TCPReceiver> logger) : IReceiver
{
    private bool exit = false;

    private Thread thread;

    private IPAddress iPAddress;

    private int port = options.Value.ServerPort;

    public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

    public IPAddress IPAddress
    {
        get
        {
            return this.iPAddress;
        }

        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(this.IPAddress), "The specified parameter must not be null.");
            }

            this.iPAddress = value;
        }
    }

    public void Start()
    {
        if (this.thread != null && this.thread.IsAlive)
        {
            throw new InvalidOperationException($"The {nameof(TCPReceiver)} can not be startet it is already running!");
        }

        this.thread = new Thread(this.Worker)
        {
            Name = $"{nameof(TCPReceiver)}"
        };

        this.exit = false;
        this.thread.Start();
        this.thread.IsBackground = false;
    }

    public void Stop()
    {
        if (this.thread == null || !this.thread.IsAlive)
        {
            throw new InvalidOperationException($"The {nameof(TCPReceiver)} can not " +
                $"be stopped it is not running yet!");
        }

        this.exit = true;
    }

    private async void Worker()
    {
        CancellationToken cancellationToken = default;
        TcpListener listener = new(IPAddress.Any, port);
        logger.LogInformation("Listener started on port {port}", port);
        listener.Start();

        while (!exit)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
            logger.LogInformation("Client connected with address and port: {endpoint}", client.Client.RemoteEndPoint);

            new Thread(() =>
            {
                ListenForMessages(client);
            }).Start();
        }
    }

    private async void ListenForMessages(TcpClient client, CancellationToken cancellationToken = default)
    {
        try
        {
            Memory<byte> buffer = new byte[4096].AsMemory();
            string? line;
            using var stream = client.GetStream();
            int bytesRead = await stream.ReadAsync(buffer, cancellationToken);
            string quote = Encoding.UTF8.GetString(buffer.Span[..bytesRead]);
            buffer.Span[..bytesRead].Clear();

            FireMessageReceived(new MessageReceivedEventArgs(quote, client));
        }
        catch (SocketException ex)
        {
            logger.LogError(ex, "{error}", ex.Message);
        }
    }

    protected virtual void FireMessageReceived(MessageReceivedEventArgs arguments)
    {
        if (arguments is null)
        {
            throw new ArgumentNullException(nameof(arguments), "The specified parameter must not be null.");
        }

        this.OnMessageReceived?.Invoke(this, arguments);
    }
}