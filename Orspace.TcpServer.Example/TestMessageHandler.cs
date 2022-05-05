using Orspace.TcpServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Orspace.TcpServer.Example
{
    public class TestMessageHandler : IConnectionHandler
    {
        const string test = "hello world";

        public async Task Start(TcpClient client, CancellationToken stopToken)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(Encoding.UTF8.GetBytes(test), 0, test.Length);
                stream.Close();

                client.Close();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
