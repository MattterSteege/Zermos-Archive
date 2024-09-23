using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Zermos_Web.Utilities;

public static class Ipv6ClientService
{
    public static IServiceCollection AddIpv6ClientService(this IServiceCollection services)
    {
        var handler = new SocketsHttpHandler
        {
            AllowAutoRedirect = true,
        };


        // Register the handler globally in HttpClientFactory (Startup.cs or Program.cs)
        services.AddHttpClient("ipv6Client").ConfigurePrimaryHttpMessageHandler(() => handler);
        
        //ipv6ClientWithoutRedirect
        var handlerWithoutRedirect = new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
        };

        
        services.AddHttpClient("ipv6ClientWithoutRedirect").ConfigurePrimaryHttpMessageHandler(() => handlerWithoutRedirect);
        
        return services;
    }
}