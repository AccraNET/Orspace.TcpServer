using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orspace.TcpServer.Models;
using Orspace.TcpServer.Services;

var hostbuiler = Host.CreateDefaultBuilder();

hostbuiler.ConfigureServices((context, services) =>
{
    services.Configure<ServerInfo>(context.Configuration.GetSection("orspace:tcpserver"));
    services.AddHostedService<TcpServerService>();
});

var host = hostbuiler.Build();

host.Run();
