using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orspace.TcpServer.Models;
using Orspace.TcpServer.Services;
using Orspace.TcpServer.Hosting;

var hostbuiler = Host.CreateDefaultBuilder();

hostbuiler.ConfigureServices((context, services) =>
{
    services.AddOrspaceTcpServer(context.Configuration);
});

var host = hostbuiler.Build();

host.Run();
