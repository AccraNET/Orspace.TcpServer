﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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


        public TcpServerService(ILogger<TcpServerService> logger, IOptions<ServerInfo> info, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _serverInfo = info.Value;
            _AppStoptoken = applicationLifetime.ApplicationStopping;
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
                    var client = await _listener.AcceptTcpClientAsync();
                    _logger.LogInformation("Client Connected: PORT: {port}", client.Client.LocalEndPoint);

                    
                }
                catch (Exception ex)
                {

                }
            }

        }
    }
}