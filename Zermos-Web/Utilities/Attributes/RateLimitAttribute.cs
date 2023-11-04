using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly int times;
    private readonly int timeWindowInSeconds;

    /// <summary>
    /// This attribute can be used to limit the amount of requests a user can make to a specific endpoint. The limit is based on timeWindowInSeconds / times. so if you set times to 10 and timeWindowInSeconds to 60, every 6 seconds, any less than 6 seconds between requests will result in a 429 error.
    /// </summary>
    /// <param name="times">How many times a user can call the endpoint in the specified time window.</param>
    /// <param name="timeWindowInSeconds">The time window in which the user can call the endpoint the specified amount of times. In seconds</param>
    public RateLimitAttribute(int times, int timeWindowInSeconds)
    {
        this.times = times;
        this.timeWindowInSeconds = timeWindowInSeconds;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();

        var cacheKey = $"{ipAddress}_{context.ActionDescriptor.Id}_{context.HttpContext.Request.Method}";

        var cacheEntry = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>().GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(timeWindowInSeconds);
            return new RateLimitState(times, timeWindowInSeconds);
        });

        if (cacheEntry.DecrementAndCheckRateLimit())
        {
            context.Result = new ContentResult
            {
                Content = "Rate limit exceeded, you can only call this endpoint " + times + " times every " + timeWindowInSeconds + " seconds.",
                StatusCode = 429 // Too Many Requests
            };
        }
        else
        {
            await next();
        }
    }
}

public class RateLimitState
{
    private int count;
    private readonly int maxCount;
    private readonly int timeWindowInSeconds;
    private DateTime lastUpdateTime;

    public RateLimitState(int maxCount, int timeWindowInSeconds)
    {
        this.maxCount = maxCount;
        this.timeWindowInSeconds = timeWindowInSeconds;
        this.lastUpdateTime = DateTime.Now;
    }

    public bool DecrementAndCheckRateLimit()
    {
        var currentTime = DateTime.Now;
        var timeElapsed = (currentTime - lastUpdateTime).TotalSeconds;
        var timePerRequest = (double)timeWindowInSeconds / maxCount;

        if (timeElapsed >= timePerRequest)
        {
            count = Math.Max(0, count - (int)(timeElapsed / timePerRequest));
            lastUpdateTime = currentTime;
        }

        if (count >= maxCount)
        {
            return true;
        }

        count++;
        return false;
    }
}