using Microsoft.Extensions.Logging;
using Orspace.TcpServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Orspace.TcpServer.Example
{
    public class TestMessageHandler : IConnectionHandler, IDisposable
    {
        const string test = "hello world (TCP)";
        private ILogger<IConnectionHandler> _logger;

        public TestMessageHandler(ILogger<IConnectionHandler> logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _logger.LogInformation("Dispose Called");
        }

        public async Task Start(TcpClient client, CancellationToken stopToken)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(Encoding.UTF8.GetBytes(test + "\n"), 0, test.Length + 1);

                //Remote IP address
                string remoteIP = (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();

                //Remote Port
                string remotePort = (client.Client.RemoteEndPoint as IPEndPoint).Port.ToString();

                await stream.WriteAsync(Encoding.UTF8.GetBytes($"Remote IP      : {remoteIP}\n"));
                await stream.WriteAsync(Encoding.UTF8.GetBytes($"Remote Port    : {remotePort}\n"));

                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error in request handler");
            }
            
        }
    }
}
