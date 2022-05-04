using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orspace.TcpServer.Models
{
    /// <summary>
    /// Class used to provide strongly typed configuration options to start the TCP server
    /// </summary>
    public class ServerInfo
    {
        public string? Name { get; set; }
        public int Port { get; set; }
        public string? IpAddress { get; set; }
        public int Backlog { get; set; } = 100;
    }
}
