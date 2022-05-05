using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orspace.TcpServer.Models;
using Orspace.TcpServer.Services;
using Orspace.TcpServer.Hosting;
using Orspace.TcpServer.Interfaces;
using Orspace.TcpServer.Example;

var hostbuiler = Host.CreateDefaultBuilder();

hostbuiler.ConfigureServices((context, services) =>
{
    services.AddOrspaceTcpServer(context.Configuration);
    services.AddMessageHandler<TestMessageHandler>();
});

var host = hostbuiler.Build();

host.Run();
