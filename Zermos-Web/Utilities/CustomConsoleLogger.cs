using System;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Zermos_Web.Utilities;

class CustomConsoleLoggerProvider : ILoggerProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomConsoleLoggerProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new CustomConsoleLogger(_httpContextAccessor);
    }

    public void Dispose()
    {
    }
}

class CustomConsoleLogger : ILogger
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomConsoleLogger(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        // Get user email
        string userEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue("email") ?? "Unknown";

        // Set color based on log level
        switch (logLevel)
        {
            case LogLevel.Trace:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case LogLevel.Debug:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case LogLevel.Information:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogLevel.Critical:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                break;
            case LogLevel.None:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
        
        Console.Write($"[{DateTime.Now:HH:mm:ss} {logLevel}]");
        if (logLevel != LogLevel.Information && logLevel != LogLevel.None)
            Console.Write($" [{userEmail}]");
        
        Console.ResetColor();
        Console.Write($" {state} {exception?.Message}");
        if (exception?.StackTrace != null)
        {
            Console.Write($"\n{exception.StackTrace}");
        }
        Console.WriteLine();
    }
}