using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // Clear any existing providers
                    logging.AddProvider(new CustomConsoleLoggerProvider()); // Use custom logger provider
                    
                    // set minimum level
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