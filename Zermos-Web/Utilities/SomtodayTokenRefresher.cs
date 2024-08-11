using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Zermos_Web.Utilities
{
    public class SomtodayTokenRefresher : IHostedService, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private Timer _timer;
        private DateTime _nextFetchTime;

        public SomtodayTokenRefresher(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Calculate the next 7-hour increment from 01-01-1970
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSinceEpoch = DateTime.UtcNow - epochStart;

            // Calculate the number of 7-hour intervals that have passed
            double hoursSinceEpoch = timeSinceEpoch.TotalHours;
            double hoursUntilNextInterval = 7 - (hoursSinceEpoch % 7);

            // Determine the next time to refresh
            _nextFetchTime = DateTime.UtcNow.AddHours(hoursUntilNextInterval);
            TimeSpan initialDelay = _nextFetchTime - DateTime.UtcNow;
            TimeSpan refreshInterval = TimeSpan.FromHours(7);

            // Log the next fetch time
            Console.WriteLine($"Next token refresh scheduled at: {_nextFetchTime:yyyy-MM-dd HH:mm:ss} UTC");

            // Start the timer to refresh tokens every 7 hours from the next increment
            _timer = new Timer(RefreshTokens, null, initialDelay, refreshInterval);
            
            return Task.CompletedTask;
        }

        private async void RefreshTokens(object state)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync("/Koppelingen/Somtoday/RefreshForAll");
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"Tokens refreshed successfully at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

                // Schedule the next fetch time and log it
                _nextFetchTime = DateTime.UtcNow.AddHours(7);
                Console.WriteLine($"Next token refresh scheduled at: {_nextFetchTime:yyyy-MM-dd HH:mm:ss} UTC");
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, etc.)
                Console.WriteLine($"An error occurred while refreshing tokens: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the timer when the service is stopped
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
