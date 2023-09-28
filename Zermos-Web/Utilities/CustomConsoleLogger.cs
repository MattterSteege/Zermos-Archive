using System;
using Microsoft.Extensions.Logging;

namespace Zermos_Web.Utilities;

class CustomConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new CustomConsoleLogger();
    }

    public void Dispose()
    {
    }
}

class CustomConsoleLogger : ILogger
{
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
        //make colored log entries
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
        Console.ResetColor();
        Console.WriteLine($" {formatter(state, exception)}");
    }
}