using Microsoft.Extensions.Logging;
using Orspace.TcpServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Orspace.TcpServer.Example
{
    public class TestMessageHandler : IConnectionHandler, IDisposable
    {
        const string test = "hello world";
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
                await stream.WriteAsync(Encoding.UTF8.GetBytes(test), 0, test.Length);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error in request handler");
            }
            
        }
    }
}
