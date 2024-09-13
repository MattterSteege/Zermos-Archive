using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Zermos_Web.Utilities;

namespace Zermos_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpContextAccessor();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders(); // Clear any existing providers
                    
                    // Use custom logger provider
                    logging.Services.AddSingleton<ILoggerProvider>(sp =>
                    {
                        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                        return new CustomConsoleLoggerProvider(httpContextAccessor);
                    });
                    
                    // Set minimum level
#if DEBUG
                    logging.SetMinimumLevel(LogLevel.Debug);
#elif RELEASE
                    logging.SetMinimumLevel(LogLevel.Warning);
#endif
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}