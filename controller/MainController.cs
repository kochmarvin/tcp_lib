using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using netlib.eventargs;
using netlib.interfaces;
using netlib.records;
using netlib.services;

namespace netlib.controller;

internal class MainController(IMessageService messageService, IReceiver receiver, ISender sender, ILogger<MainController> logger, IOptions<MainControllerOptions> options)
{
    private Thread thread;
    public void Start()
    {

        logger.LogInformation("[Controller] Controller started");

        try
        {
            receiver.Start();
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException($"The {nameof(MainController)} can not be startet it is already running!");
        }

        receiver.OnMessageReceived += this.MessageReceivedCallback;
        this.thread = new Thread(new ThreadStart(this.Worker));
        this.thread.Start();
    }


    private void MessageReceivedCallback(object? sender, MessageReceivedEventArgs arguments)
    {
        if (sender is null)
        {
            throw new ArgumentNullException(nameof(sender), "The specified parameter must not be null.");
        }

        if (arguments is null)
        {
            throw new ArgumentNullException(nameof(arguments), "The specified parameter must not be null.");
        }

        string message = arguments.Message;

        try
        {
            logger.LogInformation($"[Controller] Message reeceived: {message}");
            this.HandleMessages(message, (IPEndPoint)arguments.Sender.Client.RemoteEndPoint);
        }
        catch (SocketException)
        {
        }
    }

    private void Worker()
    {
        while (true)
        {
            // falls man was jede sekunde oder so senden muss dies das anananas
        }
    }

    private void HandleMessages(string message, IPEndPoint from)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message), "The specified parameter must not be null.");
        }

        if (from is null)
        {
            throw new ArgumentNullException(nameof(from), "The specified parameter must not be null.");
        }

        // LOGIC REINSCHREIBEN WAS PASSIREN SOLL WENN ICH WAS BEKOMM
    }
}