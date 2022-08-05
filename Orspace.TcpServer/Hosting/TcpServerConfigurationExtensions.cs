using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orspace.TcpServer.Services;
using Orspace.TcpServer.Models;
using Orspace.TcpServer.Interfaces;

namespace Orspace.TcpServer.Hosting
{
    public static class TcpServerConfigurationExtensions
    {
        public static IServiceCollection AddOrspaceTcpServer(this IServiceCollection services, IConfiguration config)
        {
            services.AddHostedService<TcpServerService>();
            services.Configure<ServerInfo>(config.GetSection("orspace:tcpserver"));

            return services;
        }

        public static IServiceCollection AddTcpMessageHandler<T>(this IServiceCollection services)
        {
            //Check if the Passed in type is really implements the IConnectionHandler interface.
            //Throw an exception if it does not implement said interface

            if (typeof(IConnectionHandler).IsAssignableFrom(typeof(T)) && typeof(T).IsClass)
            {
                services.AddScoped(typeof(IConnectionHandler), typeof(T));
                return services;
            }
            else
            {
                throw new InvalidCastException("The provided MessageHandler type does not implement IConnectionhandler");
            }
        }

    }
}
