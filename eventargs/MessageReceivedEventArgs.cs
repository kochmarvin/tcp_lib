using System.Net;
using System.Net.Sockets;

namespace netlib.eventargs;

internal class MessageReceivedEventArgs
{
    private string message;
    private TcpClient sender;
    public MessageReceivedEventArgs(string message, TcpClient sender)
    {
        this.Message = message;
        this.Sender = sender;
    }

    public string Message
    {
        get
        {
            return this.message;
        }

        private set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(this.Message), "The specified parameter must not be null.");
            }

            this.message = value;
        }
    }

    public TcpClient Sender
    {
        get
        {
            return this.sender;
        }

        private set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(this.Sender), "The specified parameter must not be null.");
            }

            this.sender = value;
        }
    }
}