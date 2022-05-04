using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Orspace.TcpServer.Interfaces
{
    public interface IConnectionHandler
    {
        public Task Start(TcpClient client, CancellationToken stopToken);
    }
}
