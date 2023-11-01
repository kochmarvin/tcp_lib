using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using netlib.controller;
using netlib.interfaces;
using netlib.records;
using netlib.services;
using netlib.tcp;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole();

// Config files
builder.Services.Configure<TCPReceiverOptions>(builder.Configuration.GetSection("TCPReceiver"));
builder.Services.Configure<TCPSenderOptions>(builder.Configuration.GetSection("TCPSender"));
builder.Services.Configure<MainControllerOptions>(builder.Configuration.GetSection("MainController"));

// Injections
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddTransient<ISender, TCPSender>();
builder.Services.AddTransient<IReceiver, TCPReceiver>();
builder.Services.AddSingleton<MainController>();

using var host = builder.Build();

MainController controller = host.Services.GetRequiredService<MainController>();
controller.Start();

Console.ReadLine();