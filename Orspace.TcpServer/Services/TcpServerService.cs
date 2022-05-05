using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orspace.TcpServer.Interfaces;
using Orspace.TcpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Orspace.TcpServer.Services
{
    public class TcpServerService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ServerInfo _serverInfo;
        private TcpListener? _listener;
        private CancellationToken _AppStoptoken;
        private Task? _serverTask;
        private IServiceProvider _serviceProvider;
        private int _connectionCount;


        public TcpServerService(ILogger<TcpServerService> logger, IOptions<ServerInfo> info, IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serverInfo = info.Value;
            _AppStoptoken = applicationLifetime.ApplicationStopping;
            _serviceProvider = serviceProvider;
            _connectionCount = 0;
        }



        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting the TCP server.");

                //We start the tcp server here
                int port = _serverInfo.Port;
                IPAddress iPAddress = IPAddress.Parse(_serverInfo.IpAddress);

                _listener = new TcpListener(iPAddress, port);

                //Start the server task
                _serverTask = Task.Run(async () => await MainServerTask(_AppStoptoken), _AppStoptoken);
            }
            catch(Exception ex)
            {
                _logger.LogError("Could Not start the Tcp Server. \n MESSEAGE: {message}", ex.Message);
            }            
        }



        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //Stop the TcpServer gracefully
            _serverTask?.Wait();
            _listener?.Stop();                       
        }



        private async Task MainServerTask(CancellationToken cancellationToken)
        {
            if(_listener != null)
            {
                _listener.Start(_serverInfo.Backlog);
                _logger.LogInformation("Tcp server started !");
            }
            else
            {
                _logger.LogError("Could not start server");
                throw new Exception();
            }
            

            while(cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync(_AppStoptoken);
                    _connectionCount++;
                    client.LingerState.LingerTime = 0;

                    _logger.LogInformation("Client Connected: \nPORT: {port} \nIP: {ip}", (client.Client.RemoteEndPoint as IPEndPoint).Port, (client.Client.RemoteEndPoint as IPEndPoint).Address);


                    try
                    {
                        IConnectionHandler? handler = _serviceProvider.GetRequiredService<IConnectionHandler>();

                        //Fire and forget
                        _ = Task.Run(async () => await RequestHandlerTask(handler, client, _AppStoptoken));
                    }
                    catch (Exception ex)
                    {
                        client.Close();
                        client.Dispose();
                        _logger.LogError(ex.Message);
                    }                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    
                }
            }

        }


        /// <summary>
        /// Handles a connection from a client.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="client"></param>
        /// <param name="stopToken"></param>
        /// <returns></returns>
        private async Task RequestHandlerTask(IConnectionHandler handler, TcpClient client, CancellationToken stopToken)
        {
            try
            {
                await handler.Start(client, stopToken);
                client.Close();
                client.Dispose();


                //Check if handler class implements the IDisposable or IDisposableAsync Interface
                //If it does, call the Dispose() or DisposeAsync() methods
                if(handler != null)
                {
                    if (typeof(IDisposable).IsAssignableFrom(handler.GetType()))
                    {
                        (handler as IDisposable).Dispose();
                    }
                    else
                    {
                        if (typeof(IAsyncDisposable).IsAssignableFrom(handler.GetType()))
                        {
                            await (handler as IAsyncDisposable).DisposeAsync();
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                _logger.LogError("Request Handler Exception \n {message}", ex.Message);
            }            
        }
    }
}
